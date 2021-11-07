using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using CodeNameK.DataContracts;
using CodeWithSaar;
using Microsoft.Extensions.Logging;

namespace CodeNameK.DataAccess
{
    public class DataRepo : IDataPointRepo, ICategoryRepo
    {
        private const string DataFolderName = "Data";
        private readonly string _baseDirectory;
        private readonly IDataWriter<DataPoint> _dataPointWriter;
        private readonly IDataPointPathService _pathService;
        private readonly ILogger _logger;

        public DataRepo(
            IDataWriter<DataPoint> dataPointWriter,
            IDataPointPathService pathService,
            ILogger<DataRepo> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _baseDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DataFolderName);
            _dataPointWriter = dataPointWriter ?? throw new ArgumentNullException(nameof(dataPointWriter));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
        }

        public Task<string> AddCategoryAsync(Category category)
        {
            if (string.IsNullOrEmpty(category?.Id))
            {
                throw new ArgumentNullException(nameof(category));
            }

            string directoryName = _pathService.GetDirectoryName(category);
            Directory.CreateDirectory(Path.Combine(_baseDirectory, directoryName));
            return Task.FromResult(category.Id);
        }

        public async Task<Guid> AddPointAsync(DataPoint newPoint)
        {
            if (newPoint is null)
            {
                throw new ArgumentNullException(nameof(newPoint));
            }

            await _dataPointWriter.WriteAsync(newPoint, _pathService.GetRelativePath(newPoint, _baseDirectory)).ConfigureAwait(false);
            return newPoint.Id;
        }

        public async Task<bool> DeletePointAsync(DataPoint dataPoint)
        {
            string filePath = _pathService.GetRelativePath(dataPoint, _baseDirectory);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Can't locate data point file.", filePath);
            }

            dataPoint.IsDeleted = true;
            await _dataPointWriter.WriteAsync(dataPoint, _pathService.GetRelativePath(dataPoint, _baseDirectory)).ConfigureAwait(false);
            return true;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            IEnumerable<string> categoryEnum = Directory.EnumerateDirectories(_baseDirectory).Select(item => Path.GetFileName(item));
            foreach (Category category in categoryEnum.Select(item =>
            {
                Category category = new Category()
                {
                    Id = FileUtility.Decode(item),
                    Values = Enumerable.Empty<DataPoint>(),
                };
                return category;
            }))
            {
                yield return category;
            }
        }

        public async IAsyncEnumerable<DataPoint> GetPointsAsync(Category category, int? year, int? month)
        {
            if (category?.Id is null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            string searchPrefix = Path.GetFullPath(Path.Combine(_baseDirectory, _pathService.GetDirectoryName(category)));
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