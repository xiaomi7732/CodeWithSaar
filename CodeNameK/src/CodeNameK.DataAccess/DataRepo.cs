using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Logging;

namespace CodeNameK.DataAccess
{
    public class DataRepo : IDataPointRepo, ICategoryRepo
    {
        private const string DataFolderName = "Data";
        private readonly string _baseDirectory;
        private readonly IDataPointWriter _dataPointWriter;
        private readonly ILogger _logger;

        public DataRepo(
            IDataPointWriter dataPointWriter,
            ILogger<DataRepo> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _baseDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DataFolderName);
            _dataPointWriter = dataPointWriter ?? throw new ArgumentNullException(nameof(dataPointWriter));
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

            await _dataPointWriter.WriteDataPointAsync(newPoint, _baseDirectory).ConfigureAwait(false);
            return newPoint.Id;
        }

        public async Task<bool> DeletePointAsync(DataPoint dataPoint)
        {
            string filePath = Path.Combine(_baseDirectory, dataPoint.GetRelativePath());
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Can't locate data point file.", filePath);
            }

            dataPoint.IsDeleted = true;
            await _dataPointWriter.WriteDataPointAsync(dataPoint, _baseDirectory).ConfigureAwait(false);
            return true;
        }

        public IAsyncEnumerable<Category> GetAllCategories()
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<DataPoint> GetPointsAsync(Category category, int? year, int? month)
        {
            if (category?.Id is null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            string searchPrefix = Path.GetFullPath(Path.Combine(_baseDirectory, category.Id));
            if (year.HasValue)
            {
                searchPrefix = Path.Combine(searchPrefix, year.Value.ToString("N4"));

                if (month.HasValue)
                {
                    searchPrefix = Path.Combine(searchPrefix, month.Value.ToString("N2"));
                }
            }

            _logger.LogInformation("Data searching prefix: {prefix}", searchPrefix);
            DirectoryInfo searchBase = new DirectoryInfo(searchPrefix);
            foreach (FileInfo file in searchBase.EnumerateFiles("*" + Constants.DataPointFileExtension, new EnumerationOptions() { RecurseSubdirectories = true }))
            {
                using (Stream input = File.OpenRead(file.FullName))
                {
                    DataPoint dataPoint = await JsonSerializer.DeserializeAsync<DataPoint>(input).ConfigureAwait(false);
                    if (!dataPoint.IsDeleted)
                    {
                        yield return dataPoint;
                    }
                }
            }
        }

        public Task<bool> UpdatePointAsync(DataPoint contract, Category category)
        {
            throw new NotImplementedException();
        }
    }
}