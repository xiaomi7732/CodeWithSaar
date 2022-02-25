using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DataContracts;
using CodeWithSaar;
using Microsoft.Extensions.Logging;

namespace CodeNameK.DAL
{
    public class DataRepo : IDataPointRepo, ICategoryRepo
    {
        private readonly IDataWriter<DataPoint> _dataPointWriter;
        private readonly ILocalPathProvider _localPathService;
        private readonly ILogger _logger;

        public DataRepo(
            IDataWriter<DataPoint> dataPointWriter,
            IDataReader<DataPoint> dataPointReader,
            ILocalPathProvider pathService,
            ILogger<DataRepo> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataPointWriter = dataPointWriter ?? throw new ArgumentNullException(nameof(dataPointWriter));
            _localPathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
        }

        public Task<string> AddCategoryAsync(Category category, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(category?.Id))
            {
                throw new ArgumentNullException(nameof(category));
            }

            string directoryName = _localPathService.GetDirectoryName(category);

            Directory.CreateDirectory(Path.Combine(_localPathService.BasePath, directoryName));
            return Task.FromResult(category.Id);
        }

        public async Task<DataPointPathInfo> AddPointAsync(DataPoint newPoint, CancellationToken cancellationToken)
        {
            if (newPoint is null)
            {
                throw new ArgumentNullException(nameof(newPoint));
            }

            string filePath = _localPathService.GetLocalPath(newPoint);
            await _dataPointWriter.WriteAsync(
                newPoint,
                filePath,
                cancellationToken).ConfigureAwait(false);
            return newPoint!;
        }

        public Task<bool> DeletePointAsync(DataPoint dataPoint, CancellationToken cancellationToken)
        {
            string filePath = _localPathService.GetLocalPath(dataPoint);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Can't locate data point file.", filePath);
            }

            string deleteMark = _localPathService.GetDeletedMarkerFilePath(dataPoint);
            if (File.Exists(deleteMark))
            {
                throw new InvalidOperationException($"The target data point has already been deleted. Marking file: {deleteMark}");
            }

            _dataPointWriter.WriteEmpty(deleteMark);
            return Task.FromResult(true);
        }

        public IEnumerable<Category> GetAllCategories()
        {
            if (!Directory.Exists(_localPathService.BasePath))
            {
                // Target folder doesn't exist;
                yield break;
            }

            IEnumerable<string> categoryEnum = Directory.EnumerateDirectories(_localPathService.BasePath).Select(item => Path.GetFileName(item));
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

            string searchPrefix = Path.GetFullPath(Path.Combine(_localPathService.BasePath, _localPathService.GetDirectoryName(category)));
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
                    string deletedMarkPath = _localPathService.GetDeletedMarkerFilePath(dataPoint!);
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

            if (File.Exists(_localPathService.GetDeletedMarkerFilePath(newPoint)))
            {
                // The target point has been deleted already;
                return false;
            }

            string originalPointPath = _localPathService.GetLocalPath(originalPointLocator);
            if (!File.Exists(originalPointPath))
            {
                throw new InvalidOperationException($"Original point doesn't exist for updating. Path: {originalPointPath}");
            }

            DataPointPathInfo newPointHandle = await AddPointAsync(newPoint, cancellationToken).ConfigureAwait(false);
            try
            {
                await DeletePointAsync(originalPointLocator, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // If delete fails, the newly added point should be removed to avoid duplicated data points.
                _dataPointWriter.Delete(_localPathService.GetLocalPath(newPointHandle));
                throw;
            }

            return true;
        }
    }
}