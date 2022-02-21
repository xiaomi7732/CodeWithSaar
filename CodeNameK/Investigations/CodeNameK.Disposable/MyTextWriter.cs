namespace LearnDisposable
{
    public class MyTextWriter : IDisposable
    {
        private readonly string _fileName;

        private Stream? _outputStream;
        private StreamWriter? _streamWriter;
        private bool _disposed;

        public MyTextWriter(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException($"'{nameof(fileName)}' cannot be null or empty.", nameof(fileName));
            }

            _fileName = fileName;

            _outputStream = File.OpenWrite(fileName);
            _streamWriter = new StreamWriter(_outputStream);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            Disposing();
        }

        protected virtual void Disposing()
        {
            _streamWriter?.Dispose();
            _streamWriter = null;
            _outputStream?.Dispose();
            _outputStream = null;
        }

        public void WriteMessage(string message)
        {
            _streamWriter?.WriteLine(message);
        }
    }
}