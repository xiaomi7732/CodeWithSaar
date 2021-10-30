using System;
using CodeNameK.DataContracts;

namespace CodeNameK.DataAccess
{
    internal static class DataPointExtensions
    {
        /// <summary>
        /// Gets the relative path to a data point.
        /// </summary>
        public static string GetRelativePath(this DataPoint dataPoint)
        {
            if (dataPoint is null)
            {
                throw new ArgumentNullException(nameof(dataPoint));
            }

            if (string.IsNullOrEmpty(dataPoint.Category?.Id))
            {
                throw new ArgumentNullException(nameof(dataPoint.Category));
            }

            return $"{dataPoint.Category.Id}/{dataPoint.WhenUTC:yyyy}/{dataPoint.WhenUTC:MM}/{dataPoint.Id:D}";
        }
    }
}