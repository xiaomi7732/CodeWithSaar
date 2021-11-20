using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.DataAccess;
using CodeNameK.DataContracts;

namespace CodeNameK.Biz;
internal class BizDataPoint : IDataPoint
{
    private readonly IDataPointRepo _dataPointRepo;

    public BizDataPoint(IDataPointRepo dataPointRepo)
    {
        _dataPointRepo = dataPointRepo ?? throw new System.ArgumentNullException(nameof(dataPointRepo));
    }

    public async Task<OperationResult<DataPoint>> AddAsync(DataPoint newPoint, CancellationToken cancellationToken)
    {
        // Business logic: new datapoint shall have a new guid
        newPoint = newPoint with { Id = Guid.NewGuid() };

        // Business logic: WhenUTC will be specified using current date time if not specificed.
        if (newPoint.WhenUTC == default)
        {
            newPoint = newPoint with { WhenUTC = DateTime.UtcNow };
        }

        // Business logic: new data point has to have a category.
        if (string.IsNullOrEmpty(newPoint.Category?.Id))
        {
            return new OperationResult<DataPoint>()
            {
                Entity = newPoint,
                IsSuccess = false,
                Reason = "Category is required for a data point.",
            };
        }

        // Business logic: no duplicated data point
        // TODO: Add the logic
        await foreach (DataPoint conflictCandidate in _dataPointRepo.GetPointsAsync(newPoint.Category, newPoint.WhenUTC.Year, newPoint.WhenUTC.Month, cancellationToken))
        {
            if (conflictCandidate.WhenUTC == newPoint.WhenUTC && conflictCandidate.Value == newPoint.Value)
            {
                return new OperationResult<DataPoint>()
                {
                    Entity = newPoint,
                    IsSuccess = false,
                    Reason = $"The data point in category {newPoint.Category.Id} that has a value of {newPoint.Value} by {newPoint.WhenUTC.ToLocalTime()} already exists.",
                };
            }
        }

        DataPointInfo newDataPointInfo = await _dataPointRepo.AddPointAsync(newPoint, cancellationToken).ConfigureAwait(false);
        return new OperationResult<DataPoint>()
        {
            Entity = newPoint,
            IsSuccess = true,
        };
    }

    public Task<OperationResult<bool>> DeleteAsync(DataPoint dataPoint, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task<OperationResult<DataPoint>> Update(DataPoint oldDataPoint, DataPoint newDataPoint, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public async IAsyncEnumerable<DataPoint> GetDataPoints(Category category, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Business Logic: category.id can't be null.
        if (string.IsNullOrEmpty(category.Id))
        {
            yield break;
        }

        await foreach (DataPoint dataPoint in _dataPointRepo.GetPointsAsync(category, year: null, month: null, cancellationToken).ConfigureAwait(false))
        {
            yield return dataPoint;
        }
    }
}