using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.DataContracts;

namespace CodeNameK.DAL
{
    /// <summary>
    /// Physical data file manipulation service.
    /// </summary>
    public class DataPointOperator : IDataWriter<DataPoint>, IDataReader<DataPoint>
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        public DataPointOperator()
        {
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public void Delete(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
            }

            File.Delete(filePath);
        }

        public async Task<DataPoint?> ReadAsync(string filePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
            }

            DataPoint? entity = null;
            using (Stream utf8JsonStream = File.OpenRead(filePath))
            {
                entity = await JsonSerializer.DeserializeAsync<DataPoint>(
                    utf8JsonStream, _jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
            }

            return entity;
        }

        public async Task WriteAsync(DataPoint data, string filePath, CancellationToken cancellationToken)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
            }

            string tempFile = Path.GetTempFileName();
            try
            {
                using (Stream output = File.OpenWrite(tempFile))
                {
                    await JsonSerializer.SerializeAsync(output, data, _jsonSerializerOptions).ConfigureAwait(false);
                }

                string? parentDirectory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(parentDirectory))
                {
                    Directory.CreateDirectory(parentDirectory);
                }
                File.Copy(tempFile, filePath, true);
                File.Delete(tempFile);
            }
            finally
            {
                // Best effort.
                _ = Task.Run(() => File.Delete(tempFile));
            }
        }

        public void WriteEmpty(string filePath)
        {
            using Stream fileStream = File.OpenWrite(filePath);
        }
    }
}