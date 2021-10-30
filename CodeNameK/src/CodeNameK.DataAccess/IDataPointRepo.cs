using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeNameK.DataContracts;

namespace CodeNameK.DataAccess
{
    public interface IDataPointRepo
    {
        /// <summary>
        /// Creates a data point and persistents it.
        /// </summary>
        Task<Guid> AddPointAsync(DataPoint newPoint);

        /// <summary>
        /// Gets a collection of data points for a given category, year and month.
        /// </summary>
        IAsyncEnumerable<DataPoint> GetPointsAsync(Category category, int? year, int? month);

        /// <summary>
        /// Updates a given point.
        /// </summary>
        Task<bool> UpdatePointAsync(DataPoint contract, Category category);

        /// <summary>
        /// Deletes a data point by id.
        /// </summary>
        Task<bool> DeletePointAsync(DataPoint dataPoint);
    }
}