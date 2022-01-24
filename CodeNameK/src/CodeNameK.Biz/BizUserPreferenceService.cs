using System;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.Core.Utilities;
using CodeNameK.DAL.Interfaces;
using Microsoft.Extensions.Options;

namespace CodeNameK.BIZ
{
    public sealed class BizUserPreferenceService : IBizUserPreferenceService, IDisposable
    {
        public const string FilePath = "UserPreference.json";
        private bool _isDisposed = false;

        private readonly IOptionsMonitor<UserPreference> _currentUserPreference;
        private readonly IUserPreferenceManager _userPreferenceManager;

        private IDisposable? _userPreferenceChangedListener;

        public BizUserPreferenceService(
            IOptionsMonitor<UserPreference> currentUserPreference,
            IUserPreferenceManager userPreferenceManager)
        {
            _currentUserPreference = currentUserPreference ?? throw new System.ArgumentNullException(nameof(currentUserPreference));
            _userPreferenceManager = userPreferenceManager ?? throw new System.ArgumentNullException(nameof(userPreferenceManager));

            _userPreferenceChangedListener = _currentUserPreference.OnChange(OnUserPreferenceChanged);
        }

        public UserPreference UserPreference => _currentUserPreference.CurrentValue;

        public event EventHandler<UserPreference>? UserPreferenceChanged;

        public Task DisableSyncAsync(CancellationToken cancellationToken = default)
        {
            UserPreference current = UserPreference;
            if (current.EnableSync)
            {
                current.EnableSync = false;
                return _userPreferenceManager.WriteAsync(current, FilePath, cancellationToken);
            }
            return Task.CompletedTask;
        }

        public Task EnableSyncAsync(CancellationToken cancellationToken = default)
        {
            UserPreference current = UserPreference;
            if (!current.EnableSync)
            {
                current.EnableSync = true;
                return _userPreferenceManager.WriteAsync(current, FilePath, cancellationToken);
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;

            _userPreferenceChangedListener?.Dispose();
            _userPreferenceChangedListener = null;

            _userPreferenceManager?.TryDispose();
        }

        private void OnUserPreferenceChanged(UserPreference newValue, string a)
        {
            UserPreferenceChanged?.Invoke(this, newValue);
        }
    }
}