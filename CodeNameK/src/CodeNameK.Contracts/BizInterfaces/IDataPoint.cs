using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.Contracts;
using CodeNameK.DataContracts;

namespace CodeNameK.BIZ.Interfaces;

public interface IDataPoint
{
    IAsyncEnumerable<DataPoint> GetDataPoints(Category category, DateTime? startDateTimeUtc = null, DateTime? endDateTimeUtc = null, CancellationToken cancellationToken = default);
    Task<OperationResult<DataPoint>> AddAsync(DataPoint newPoint, CancellationToken cancellationToken);
    Task<OperationResult<bool>> DeleteAsync(DataPoint dataPoint, CancellationToken cancellationToken);
    Task<OperationResult<DataPoint>> Update(DataPoint oldDataPoint, DataPoint newDataPoint, CancellationToken cancellationToken);
}