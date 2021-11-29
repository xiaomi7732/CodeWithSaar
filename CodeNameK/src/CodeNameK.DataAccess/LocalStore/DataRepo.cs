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
        private readonly ILocalPathProvider _localPathService;
        private readonly ILogger _logger;

        public DataRepo(
            IDataWriter<DataPoint> dataPointWriter,
            IDataReader<DataPoint> dataPointReader,
            ILocalPathProvider pathService,
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
            _localPathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
        }

        public Task<string> AddCategoryAsync(Category category, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(category?.Id))
            {
                throw new ArgumentNullException(nameof(category));
            }

            string directoryName = _localPathService.GetDirectoryName(category);
            Directory.CreateDirectory(Path.Combine(_baseDirectory, directoryName));
            return Task.FromResult(category.Id);
        }

        public async Task<DataPointPathInfo> AddPointAsync(DataPoint newPoint, CancellationToken cancellationToken)
        {
            if (newPoint is null)
            {
                throw new ArgumentNullException(nameof(newPoint));
            }

            string filePath = _localPathService.GetLocalPath(newPoint, _baseDirectory);
            await _dataPointWriter.WriteAsync(
                newPoint,
                filePath,
                cancellationToken).ConfigureAwait(false);
            return newPoint!;
        }

        public Task<bool> DeletePointAsync(DataPoint dataPoint, CancellationToken cancellationToken)
        {
            string filePath = _localPathService.GetLocalPath(dataPoint, _baseDirectory);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Can't locate data point file.", filePath);
            }

            string deleteMark = _localPathService.GetDeletedMarkerFilePath(dataPoint, _baseDirectory);
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

            string searchPrefix = Path.GetFullPath(Path.Combine(_baseDirectory, _localPathService.GetDirectoryName(category)));
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
                    string deletedMarkPath = _localPathService.GetDeletedMarkerFilePath(dataPoint!, _baseDirectory);
                    if (!File.Exists(deletedMarkPath))
                    {
                        yield return dataPoint;
                    }
                }
            }
        }

        public async Task<bool> UpdatePointAsync(DataPoint originalPointLocator, DataPoint newPoint, CancellationToken cancellationToken)
        {
            if (originalPointLocator is null)
            {
                throw new ArgumentNullException(nameof(originalPointLocator));
            }

            if (File.Exists(_localPathService.GetDeletedMarkerFilePath(newPoint, _baseDirectory)))
            {
                // The target point has been deleted already;
                return false;
            }

            string originalPointPath = _localPathService.GetLocalPath(originalPointLocator, _baseDirectory);
            if (!File.Exists(originalPointPath))
            {
                throw new InvalidOperationException("Original point doesn't exist for updating.");
            }

            DataPointPathInfo newPointHandle = await AddPointAsync(newPoint, cancellationToken).ConfigureAwait(false);
            try
            {
                await DeletePointAsync(originalPointLocator, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // If delete fails, the newly added point should be removed to avoid duplicated data points.
                _dataPointWriter.Delete(_localPathService.GetLocalPath(newPointHandle, _baseDirectory));
                throw;
            }

            return true;
        }
    }
}