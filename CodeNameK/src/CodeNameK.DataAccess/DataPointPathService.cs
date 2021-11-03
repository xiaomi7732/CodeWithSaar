using System;
using System.IO;
using CodeNameK.DataContracts;

namespace CodeNameK.DataAccess
{
    public class DataPointPathService : IDataPointPathService
    {
        public const string DataPointFileExtension = ".dpt";

        public string GetRelativePath(DataPoint dataPoint, string baseDirectory = null)
        {
            if (dataPoint is null)
            {
                throw new ArgumentNullException(nameof(dataPoint));
            }

            if (string.IsNullOrEmpty(dataPoint.Category?.Id))
            {
                throw new ArgumentNullException(nameof(dataPoint.Category));
            }


            string relativePath = $"{dataPoint.Category.Id}/{dataPoint.WhenUTC:yyyy}/{dataPoint.WhenUTC:MM}/{dataPoint.Id:D}{DataPointFileExtension}";
            if (!string.IsNullOrEmpty(baseDirectory))
            {
                relativePath = Path.Combine(baseDirectory, relativePath);
            }
            return relativePath;
        }
    }
}