using System;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.DAL.OneDrive;

namespace CodeNameK.BIZ
{
    internal class BizSync : ISync
    {
        private readonly IOneDriveSync _oneDriveSync;

        public BizSync(IOneDriveSync oneDriveSync)
        {
            _oneDriveSync = oneDriveSync ?? throw new System.ArgumentNullException(nameof(oneDriveSync));
        }

        public Task<int> SyncUp(IProgress<double> progress, CancellationToken cancellationToken)
        {
            return _oneDriveSync.UpSyncAsync(progress, cancellationToken);
        }
    }
}