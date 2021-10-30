using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using CodeNameK.DataContracts;

namespace CodeNameK.DataAccess
{
    public class DataRepo : IDataPointRepo, ICategoryRepo
    {
        private const string DataFolderName = "Data";
        private readonly string _baseDirectory;
        public DataRepo()
        {
            _baseDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DataFolderName);
        }

        public Task<string> AddCategoryAsync(Category category)
        {
            if (string.IsNullOrEmpty(category?.Id))
            {
                throw new ArgumentNullException(nameof(category));
            }

            Directory.CreateDirectory(Path.Combine(_baseDirectory, category.Id));
            return Task.FromResult(category.Id);
        }

        public async Task<Guid> AddPointAsync(DataPoint newPoint)
        {
            if (newPoint is null)
            {
                throw new ArgumentNullException(nameof(newPoint));
            }

            string targetFilePath = Path.Combine(_baseDirectory, newPoint.GetRelativePath());
            Directory.CreateDirectory(Path.GetDirectoryName(targetFilePath));
            using (Stream output = File.OpenWrite(targetFilePath))
            {
                await JsonSerializer.SerializeAsync(output, newPoint).ConfigureAwait(false);
                return newPoint.Id;
            }
        }

        public Task<bool> DeleteCategory(string categoryId)
        {
            string targetFolder = Path.Combine(_baseDirectory, categoryId);
            Directory.Delete(targetFolder, recursive: true);
            return Task.FromResult(true);
        }

        public async Task<bool> DeletePointAsync(DataPoint dataPoint)
        {
            string filePath = Path.Combine(_baseDirectory, dataPoint.GetRelativePath());
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Can't locate data point file.", filePath);
            }

            string tempFile = Path.GetTempFileName();
            try
            {
                dataPoint.IsDeleted = true;
                using (Stream output = File.OpenWrite(tempFile))
                {
                    await JsonSerializer.SerializeAsync(output, dataPoint).ConfigureAwait(false);
                }
                File.Move(tempFile, filePath, overwrite: true);
                return true;
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        public IAsyncEnumerable<Category> GetAllCategories()
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<DataPoint> GetPointsAsync(Category category, int? year, int? month)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePointAsync(DataPoint contract, Category category)
        {
            throw new NotImplementedException();
        }
    }
}