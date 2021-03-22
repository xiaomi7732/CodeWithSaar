using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeWithSaar.IPC
{
    internal class DuplexNamedPipeService : INamedPipeServerService, INamedPipeClientService, IDisposable
    {
        private SemaphoreSlim _threadSafeLock = new SemaphoreSlim(1, 1);
        private PipeStream _pipeStream;
        private readonly NamedPipeOptions _options;
        private readonly ISerializationProvider _serializer;
        private readonly ILogger _logger;
        private NamedPipeRole _currentMode = NamedPipeRole.NotSpecified;

        public string PipeName { get; private set; }

        internal DuplexNamedPipeService(IOptions<NamedPipeOptions> namedPipeOptions, ISerializationProvider serializer = null, ILogger<DuplexNamedPipeService> logger = null)
        {
            _options = namedPipeOptions?.Value ?? new NamedPipeOptions();
            _serializer = serializer ?? new DefaultSerializationProvider();
            _logger = logger;
        }

        public async Task WaitForConnectionAsync(string pipeName, CancellationToken cancellationToken)
        {
            try
            {
                using CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource(_options.ConnectionTimeout);
                using CancellationTokenSource linkedCancellatinoTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellationTokenSource.Token);
                cancellationToken = linkedCancellatinoTokenSource.Token;

                await _threadSafeLock.WaitAsync(cancellationToken).ConfigureAwait(false);
                switch (_currentMode)
                {
                    case NamedPipeRole.Client:
                        throw new InvalidOperationException("Can't wait for connection on a client.");
                    case NamedPipeRole.Server:
                        if (_pipeStream.IsConnected)
                        {
                            throw new InvalidOperationException("A connection is already established.");
                        }
                        // Wait for message again.
                        break;
                    case NamedPipeRole.NotSpecified:
                        // Establish connection for the first time.
                        PipeName = pipeName;
                        _currentMode = NamedPipeRole.Server;
                        NamedPipeServerStream serverStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, maxNumberOfServerInstances: 1, transmissionMode: PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                        _pipeStream = serverStream;
                        break;
                    default:
                        throw new NotSupportedException(FormattableString.Invariant($"Unsupported mode: {_currentMode}"));
                }

                await ((NamedPipeServerStream)_pipeStream).WaitForConnectionAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _threadSafeLock.Release();
            }
        }


        public async Task ConnectAsync(string pipeName, CancellationToken cancellationToken)
        {
            try
            {
                using CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource(_options.ConnectionTimeout);
                using CancellationTokenSource linkedCancellatinoTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellationTokenSource.Token);
                cancellationToken = linkedCancellatinoTokenSource.Token;

                await _threadSafeLock.WaitAsync(cancellationToken).ConfigureAwait(false);
                switch (_currentMode)
                {
                    case NamedPipeRole.Client:
                        if (_pipeStream.IsConnected)
                        {
                            throw new InvalidOperationException("A connection is already established.");
                        }
                        break;
                    case NamedPipeRole.Server:
                        throw new InvalidOperationException("Can't connect to another server from a server.");
                    case NamedPipeRole.NotSpecified:
                        // New connection
                        _currentMode = NamedPipeRole.Client;
                        PipeName = pipeName;
                        NamedPipeClientStream clientStream = new NamedPipeClientStream(serverName: ".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                        _pipeStream = clientStream;
                        break;
                    default:
                        throw new NotSupportedException(FormattableString.Invariant($"Unsupported mode: {_currentMode}"));
                }

                await ((NamedPipeClientStream)_pipeStream).ConnectAsync(cancellationToken).ConfigureAwait(false);

            }
            finally
            {
                _threadSafeLock.Release();
            }
        }

        public async Task<string> ReadMessageAsync(TimeSpan timeout = default, CancellationToken cancellationToken = default)
        {
            VerifyModeIsSpecified();

            timeout = VerifyReadWriteTimeout(timeout);

            using CancellationTokenSource timeoutSource = new CancellationTokenSource(timeout);
            using CancellationTokenSource linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, cancellationToken);
            cancellationToken = linkedCancellationTokenSource.Token;

            StringBuilder resultBuilder = new StringBuilder();

            Task timeoutTask = Task.Delay(timeout, cancellationToken);
            Task<string> readlineTask = Task.Run(async () =>
            {
                using (StreamReader reader = new StreamReader(_pipeStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: -1, leaveOpen: true))
                {
                    return await reader.ReadLineAsync().ConfigureAwait(false);
                }
            });

            await Task.WhenAny(timeoutTask, readlineTask).ConfigureAwait(false);

            if (!readlineTask.IsCompleted)
            {
                throw new TimeoutException($"Can't finish reading message within given timeout: {timeout.TotalMilliseconds}ms");
            }

            return readlineTask.Result;
        }

        public async Task SendMessageAsync(string message, TimeSpan timeout = default, CancellationToken cancellationToken = default)
        {
            VerifyModeIsSpecified();
            timeout = VerifyReadWriteTimeout(timeout);

            using CancellationTokenSource timeoutSource = new CancellationTokenSource(timeout);
            using CancellationTokenSource linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, cancellationToken);
            cancellationToken = linkedCancellationTokenSource.Token;

            using StreamWriter writer = new StreamWriter(_pipeStream, encoding: Encoding.UTF8, bufferSize: -1, leaveOpen: true);
            ReadOnlyMemory<char> buffer = new ReadOnlyMemory<char>(message.ToCharArray());
            await writer.WriteLineAsync(buffer, cancellationToken).ConfigureAwait(false);
        }

        public Task SendAsync<T>(T payload, TimeSpan timeout = default, CancellationToken cancellationToken = default)
        {
            if (_serializer.TrySerialize(payload, out string serialized))
            {
                return SendMessageAsync(serialized, timeout, cancellationToken);
            }
            throw new NotSupportedException("Unsupported payload for sending over named pipeline.");
        }

        public async Task<T> ReadAsync<T>(TimeSpan timeout = default, CancellationToken cancellationToken = default)
        {
            string serialized = await ReadMessageAsync(timeout, cancellationToken).ConfigureAwait(false);
            if (_serializer.TryDeserialize<T>(serialized, out T target))
            {
                return target;
            }
            throw new NotSupportedException("Failed to fetch the object over the named pipeline.");
        }

        private void VerifyModeIsSpecified()
        {
            if (_currentMode is NamedPipeRole.NotSpecified)
            {
                throw new InvalidOperationException("Pipe mode requires to be set before operations. Either wait for connection as a server or connect to a server as a client before reading or writing messages.");
            }
        }

        private TimeSpan VerifyReadWriteTimeout(TimeSpan timeout)
        {
            if (timeout == default)
            {
                timeout = _options.DefaultMessageTimeout;
            }
            if (timeout == default)
            {
                throw new InvalidOperationException("Read message timeout can't be set to 0.");
            }

            return timeout;
        }

        public void Dispose()
        {
            _threadSafeLock?.Dispose();
            _threadSafeLock = null;

            _pipeStream?.Dispose();
            _pipeStream = null;
        }

        public void Disconnect()
        {
            VerifyModeIsSpecified();

            if (_currentMode == NamedPipeRole.Server && _pipeStream.IsConnected && _pipeStream is NamedPipeServerStream serverStream)
            {
                serverStream.Disconnect();
            }
        }
    }
}
