using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CodeNameK.DataContracts;

namespace CodeNameK.DataAccess
{


    public class DataPointWriter : IDataPointWriter
    {
        public async Task WriteDataPointAsync(DataPoint newPoint, string baseDirectory)
        {
            if (newPoint is null)
            {
                throw new ArgumentNullException(nameof(newPoint));
            }

            string targetFilePath = Path.Combine(baseDirectory, newPoint.GetRelativePath());
            Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));

            string tempFile = Path.GetTempFileName();
            try
            {
                using (Stream output = File.OpenWrite(tempFile))
                {
                    await JsonSerializer.SerializeAsync(output, newPoint).ConfigureAwait(false);
                }

                File.Move(tempFile, targetFilePath, overwrite: true);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}