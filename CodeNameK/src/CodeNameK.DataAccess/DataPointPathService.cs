using System;
using System.IO;
using CodeNameK.DataContracts;
using CodeWithSaar;

namespace CodeNameK.DataAccess
{
    public class DataPointPathService : IDataPointPathService
    {
        public const string DataPointFileExtension = ".dpt";

        public string GetRelativePath(DataPoint dataPoint, string? baseDirectory = null)
        {
            if (dataPoint is null)
            {
                throw new ArgumentNullException(nameof(dataPoint));
            }

            if (string.IsNullOrEmpty(dataPoint.Category?.Id))
            {
                throw new ArgumentNullException(nameof(dataPoint.Category));
            }

            string relativePath = $"{GetDirectoryName(dataPoint.Category)}/{dataPoint.WhenUTC:yyyy}/{dataPoint.WhenUTC:MM}/{dataPoint.Id:D}{DataPointFileExtension}";
            if (!string.IsNullOrEmpty(baseDirectory))
            {
                relativePath = Path.Combine(baseDirectory, relativePath);
            }
            return relativePath;
        }

        public string GetDirectoryName(Category category)
        {
            if (category is null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (category.Id is null)
            {
                throw new ArgumentNullException(nameof(category.Id));
            }

            return FileUtility.Encode(category.Id);
        }

        public string GetDeletedMarkerFilePath(DataPoint dataPoint, string? baseDirectory = null)
        {
            string dataPointPath = GetRelativePath(dataPoint, baseDirectory);
            return Path.ChangeExtension(dataPointPath, "del");
        }
    }
}