using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CodeNameK.DataContracts;

namespace CodeNameK.DataAccess
{


    public class DataPointWriter : IDataWriter<DataPoint>
    {
        public async Task WriteAsync(DataPoint data, string filePath)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            string tempFile = Path.GetTempFileName();
            try
            {
                using (Stream output = File.OpenWrite(tempFile))
                {
                    await JsonSerializer.SerializeAsync(output, data).ConfigureAwait(false);
                }

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.Move(tempFile, filePath, overwrite: true);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}