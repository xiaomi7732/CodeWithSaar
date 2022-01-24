using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.Contracts;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.Core.Utilities;
using CodeNameK.DAL.Interfaces;

namespace CodeNameK.DAL
{
    /// <summary>
    /// An implementation that manages user <cref="CodeNameK.Contracts.UserPreference" />, primary to support writing values.
    /// One write at a time as well.
    /// </summary>
    internal sealed class UserPreferenceManager : IDataWriter<UserPreference>, IUserPreferenceManager, IDisposable
    {
        private bool _isDisposed = false;
        private readonly SemaphoreSlim _writerLock = new SemaphoreSlim(1, 1);

        private UserPreferenceManager()
        {
        }
        public static UserPreferenceManager Instance { get; } = new UserPreferenceManager();

        /// <summary>
        /// Writes the user settings to target file.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        /// <param name="cancellationToken"></param>
        public async Task WriteAsync(UserPreference data, string filePath, CancellationToken cancellationToken)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            SectionOf<UserPreference> contract = new SectionOf<UserPreference>(UserPreference.SectionName, data);
            await _writerLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                string tempWritingFile = Path.GetTempFileName();
                JsonSerializerOptions serializationOptions = new JsonSerializerOptions();
                serializationOptions.Converters.Add(new SectionOfJsonSerializer<UserPreference>());
                using (Stream output = File.OpenWrite(tempWritingFile))
                {
                    await JsonSerializer.SerializeAsync(output, contract, serializationOptions).ConfigureAwait(false);
                }
                FileUtilities.Move(tempWritingFile, filePath, true);
            }
            finally
            {
                _writerLock.Release();
            }
        }

        public void Delete(string filePath)
        {
            throw new NotSupportedException();
        }

        public void WriteEmpty(string filePath)
        {
            throw new NotSupportedException();
        }



        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                _writerLock.Dispose();
                // Usually, the SemaphoreSlim should be set to null, so that GC will take care of the managed object. But since this is a singleton that's going to be disposed at the 
                // end of the application, it doesn't matter to not set null of it.
            }
        }
    }
}