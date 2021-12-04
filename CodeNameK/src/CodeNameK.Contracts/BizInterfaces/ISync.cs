using CodeNameK.Contracts;
using CodeNameK.Contracts.DataContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeNameK.BIZ.Interfaces
{
    public interface ISync
    {
        Task<OperationResult<SyncStatistic>> Sync(IProgress<SyncProgress>? progress, CancellationToken cancellationToken = default);
    }
}