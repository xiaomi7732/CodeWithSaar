using System.Threading;
using System.Threading.Tasks;
using CodeNameK.Contracts;

namespace CodeNameK.DAL.Interfaces
{
    /// <summary>
    /// Defines what can user preference manager do.
    /// </summary>
    public interface IUserPreferenceManager
    {
        Task WriteAsync(UserPreference data, string filePath, CancellationToken cancellationToken);
    }
}