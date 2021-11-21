using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DataContracts;
using CodeWithSaar;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeNameK.DAL
{
    public class DataRepo : IDataPointRepo, ICategoryRepo
    {
        private const string DataFolderName = "Data";
        private readonly string _baseDirectory;
        private readonly IDataWriter<DataPoint> _dataPointWriter;
        private readonly IDataReader<DataPoint> _dataPointReader;
        private readonly IDataPointPathService _pathService;
        private readonly ILogger _logger;

        public DataRepo(
            IDataWriter<DataPoint> dataPointWriter,
            IDataReader<DataPoint> dataPointReader,
            IDataPointPathService pathService,
            IOptions<LocalStoreOptions> options,
            ILogger<DataRepo> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            string? localStorePath = options?.Value.DataStorePath;
            if (string.IsNullOrEmpty(localStorePath))
            {
                string? exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                _baseDirectory = string.IsNullOrEmpty(exePath) ? DataFolderName : Path.Combine(exePath, DataFolderName);
            }
            else
            {
                _baseDirectory = Environment.ExpandEnvironmentVariables(localStorePath);
            }
            _logger.LogInformation("Local Store Path: {localStorePath}", _baseDirectory);
            Directory.CreateDirectory(_baseDirectory);

            _dataPointWriter = dataPointWriter ?? throw new ArgumentNullException(nameof(dataPointWriter));
            _dataPointReader = dataPointReader ?? throw new ArgumentNullException(nameof(dataPointReader));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
        }

        public Task<string> AddCategoryAsync(Category category, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(category?.Id))
            {
                throw new ArgumentNullException(nameof(category));
            }

            string directoryName = _pathService.GetDirectoryName(category);
            Directory.CreateDirectory(Path.Combine(_baseDirectory, directoryName));
            return Task.FromResult(category.Id);
        }

        public async Task<DataPointInfo> AddPointAsync(DataPoint newPoint, CancellationToken cancellationToken)
        {
            if (newPoint is null)
            {
                throw new ArgumentNullException(nameof(newPoint));
            }

            string filePath = _pathService.GetRelativePath(newPoint, _baseDirectory);
            await _dataPointWriter.WriteAsync(
                newPoint,
                filePath,
                cancellationToken).ConfigureAwait(false);
            return new DataPointInfo()
            {
                DataPointId = newPoint.Id,
                PhysicalLocation = filePath,
            };
        }

        public Task<bool> DeletePointAsync(DataPoint dataPoint, CancellationToken cancellationToken)
        {
            string filePath = _pathService.GetRelativePath(dataPoint, _baseDirectory);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Can't locate data point file.", filePath);
            }

            string deleteMark = _pathService.GetDeletedMarkerFilePath(dataPoint, _baseDirectory);
            if (File.Exists(deleteMark))
            {
                throw new InvalidOperationException($"The target data point has already been deleted. Marking file: {deleteMark}");
            }

            _dataPointWriter.WriteEmpty(deleteMark);
            return Task.FromResult(true);
        }

        public IEnumerable<Category> GetAllCategories()
        {
            if (!Directory.Exists(_baseDirectory))
            {
                // Target folder doesn't exist;
                yield break;
            }

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

        public async IAsyncEnumerable<DataPoint> GetPointsAsync(
            Category category,
            int? year,
            int? month,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (category?.Id is null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            string searchPrefix = Path.GetFullPath(Path.Combine(_baseDirectory, _pathService.GetDirectoryName(category)));
            if (year.HasValue)
            {
                searchPrefix = Path.Combine(searchPrefix, year.Value.ToString("D4"));

                if (month.HasValue)
                {
                    searchPrefix = Path.Combine(searchPrefix, month.Value.ToString("D2"));
                }
            }

            _logger.LogInformation("Data searching prefix: {prefix}", searchPrefix);
            DirectoryInfo searchBase = new DirectoryInfo(searchPrefix);
            if (!Directory.Exists(searchBase.FullName))
            {
                // Nothing
                yield break;
            }
            foreach (FileInfo file in searchBase.EnumerateFiles("*" + Constants.DataPointFileExtension, SearchOption.AllDirectories))
            {
                using (Stream input = File.OpenRead(file.FullName))
                {
                    DataPoint? dataPoint = await JsonSerializer.DeserializeAsync<DataPoint>(input).ConfigureAwait(false);
                    if (dataPoint is null)
                    {
                        continue;
                    }
                    dataPoint = dataPoint with { Category = category };
                    string deletedMarkPath = _pathService.GetDeletedMarkerFilePath(dataPoint, _baseDirectory);
                    if (!File.Exists(deletedMarkPath))
                    {
                        yield return dataPoint;
                    }
                }
            }
        }

        public async Task<bool> UpdatePointAsync(DataPoint originalPointLocator, DataPoint newPoint, CancellationToken cancellationToken)
        {
            if (File.Exists(_pathService.GetDeletedMarkerFilePath(newPoint, _baseDirectory)))
            {
                // The target point has been deleted already;
                return false;
            }

            string originalPointPath = _pathService.GetRelativePath(originalPointLocator, _baseDirectory);
            if (!File.Exists(originalPointPath))
            {
                throw new InvalidOperationException("Original point doesn't exist for updating.");
            }

            DataPointInfo newPointHandle = await AddPointAsync(newPoint, cancellationToken).ConfigureAwait(false);
            try
            {
                await DeletePointAsync(originalPointLocator, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // If delete fails, the newly added point should be removed to avoid duplicated data points.
                _dataPointWriter.Delete(newPointHandle.PhysicalLocation);
                throw;
            }

            return true;
        }
    }
}