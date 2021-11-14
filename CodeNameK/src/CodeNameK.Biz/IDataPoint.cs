using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.DataContracts;

namespace CodeNameK.Biz;

public interface IDataPoint
{
    IAsyncEnumerable<DataPoint> GetDataPoints(Category category, CancellationToken cancellationToken);
    Task<OperationResult<DataPoint>> AddAsync(DataPoint newPoint, CancellationToken cancellationToken);
    Task<OperationResult<bool>> DeleteAsync(DataPoint dataPoint, CancellationToken cancellationToken);
    Task<OperationResult<DataPoint>> Update(DataPoint oldDataPoint, DataPoint newDataPoint, CancellationToken cancellationToken);
}