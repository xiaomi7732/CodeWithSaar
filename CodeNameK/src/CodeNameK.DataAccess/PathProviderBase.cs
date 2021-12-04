using System;
using System.Collections.Generic;
using System.IO;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DataContracts;

namespace CodeNameK.DAL;

internal abstract class PathProviderBase : IRemotePathProvider, ILocalPathProvider
{
    /// <summary>
    /// Get encoded category name for local storage.
    /// </summary>
    /// <param name="category"></param>
    public string GetDirectoryName(Category category)
    {
        if (string.IsNullOrEmpty(category?.Id))
        {
            throw new ArgumentNullException(nameof(category));
        }

        return EncodeCategory(category.Id);
    }

    /// <summary>
    /// Gets a data point entity by local or remote path.
    /// </summary>
    /// <param name="path">Relative path including the base path. For example: data/CategoryName/yyyy/MM/guid.dtp</param>
    /// <param name="basePath">The base path. For example: data.</param>
    public bool TryGetDataPointInfo(string path, out DataPointPathInfo? pathInfo)
    {
        pathInfo = null;
        path = DecodePath(path);
        if (!TryGetValidFileExtension(path, out _))
        {
            return false;
        }

        pathInfo = GetDataPointInfo(path, DecodeCategory);
        return true;
    }

    public string GetLocalPath(DataPointPathInfo dataPointInfo)
    {
        if (dataPointInfo is null)
        {
            throw new ArgumentNullException(nameof(dataPointInfo));
        }

        if (string.IsNullOrEmpty(dataPointInfo.Category?.Id))
        {
            throw new ArgumentNullException(nameof(dataPointInfo.Category));
        }

        string fullPath = Path.Combine(
            BasePath,
            GetDirectoryName(dataPointInfo.Category),
            dataPointInfo.YearFolder.ToString("D4"),
            dataPointInfo.MonthFolder.ToString("D2"),
            Path.ChangeExtension(dataPointInfo.Id.ToString("D"), dataPointInfo.IsDeletionMark ? Constants.DeletedMarkerFileExtension : Constants.DataPointFileExtension));

        return EncodePath(fullPath);
    }

    public string GetRemotePath(DataPointPathInfo dataPointInfo) => GetLocalPath(dataPointInfo);

    /// <summary>
    /// Parse a file path into DataPointPathInfo object.
    /// </summary>
    /// <param name="path">Relative path to a data point. For example: base/EncodedCategoryName/yyyy/MM/data-id-guid.ext</param>
    /// <param name="basePath">Base path. `data` for example.</param>
    /// <param name="decodeCategoryName">A delegate to decode category name for remote storage or local storage</param>
    private DataPointPathInfo GetDataPointInfo(string path, Func<string, string> decodeCategoryName)
    {
        string relativePath = Path.GetRelativePath(BasePath, path).Replace('\\', '/');
        string[] tokens = relativePath.Split("/");

        string categoryId = decodeCategoryName(tokens[0]);
        ushort year = ushort.Parse(tokens[1]);
        ushort month = ushort.Parse(tokens[2]);
        Guid id = Guid.Parse(Path.GetFileNameWithoutExtension(tokens[3]));
        string fileNameExtension = Path.GetExtension(tokens[3]);

        return new DataPointPathInfo()
        {
            Category = new Category() { Id = categoryId },
            Id = id,
            YearFolder = year,
            MonthFolder = month,
            IsDeletionMark = string.Equals(fileNameExtension, Constants.DeletedMarkerFileExtension, StringComparison.OrdinalIgnoreCase),
        };
    }

    /// <summary>
    /// Gets deleted marker file path by data point.
    /// </summary>
    public string GetDeletedMarkerFilePath(DataPointPathInfo dataPoint)
    {
        string dataPointPath = GetLocalPath(dataPoint);
        return Path.ChangeExtension(dataPointPath, Constants.DeletedMarkerFileExtension);
    }

    /// <summary>
    /// Gets the base path. The base path might be differnet depends on whether it is a service of local or remote.
    /// </summary>
    public abstract string BasePath { get; }

    private bool TryGetValidFileExtension(string path, out string extension)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
        }

        extension = Path.GetExtension(path);
        return string.Equals(extension, Constants.DataPointFileExtension, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(extension, Constants.DeletedMarkerFileExtension, StringComparison.OrdinalIgnoreCase);
    }

    protected abstract string EncodeCategory(string categoryName);
    protected abstract string DecodeCategory(string categoryName);

    protected abstract string EncodePath(string path);
    protected abstract string DecodePath(string path);

    public abstract bool PhysicalFileExists(DataPointPathInfo dataPointPathInfo);

    public abstract IEnumerable<DataPointPathInfo> ListAllDataPointPaths();

}