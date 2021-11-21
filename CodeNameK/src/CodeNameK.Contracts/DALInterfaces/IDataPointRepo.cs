using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.DataContracts;

namespace CodeNameK.DAL.Interfaces
{
    public interface IDataPointRepo
    {
        /// <summary>
        /// Creates a data point and persistents it.
        /// </summary>
        Task<DataPointInfo> AddPointAsync(DataPoint newPoint, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a collection of data points for a given category, year and month.
        /// </summary>
        IAsyncEnumerable<DataPoint> GetPointsAsync(Category category, int? year, int? month, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a given point.
        /// </summary>
        Task<bool> UpdatePointAsync(DataPoint originalPointLocator, DataPoint newCategoryPayload, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a data point by id.
        /// </summary>
        Task<bool> DeletePointAsync(DataPoint dataPoint, CancellationToken cancellationToken);
    }
}