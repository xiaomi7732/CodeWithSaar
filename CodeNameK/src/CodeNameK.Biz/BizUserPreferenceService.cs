using System.Threading;
using System.Threading.Tasks;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.DAL.Interfaces;
using Microsoft.Extensions.Options;

namespace CodeNameK.BIZ
{
    public class BizUserPreferenceService : IBizUserPreferenceService
    {
        public const string FilePath = "UserPreference.json";

        private readonly IOptionsMonitor<UserPreference> _currentUserPreference;
        private readonly IUserPreferenceManager _userPreferenceManager;

        public BizUserPreferenceService(
            IOptionsMonitor<UserPreference> currentUserPreference,
            IUserPreferenceManager userPreferenceManager)
        {
            _currentUserPreference = currentUserPreference ?? throw new System.ArgumentNullException(nameof(currentUserPreference));
            _userPreferenceManager = userPreferenceManager ?? throw new System.ArgumentNullException(nameof(userPreferenceManager));
        }

        public UserPreference UserPreference => _currentUserPreference.CurrentValue;

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
    }
}