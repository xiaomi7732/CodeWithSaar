using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeNameK.DAL.Interfaces
{
    public interface ITokenCredentialManager<T>
    {
        Task<T> SignInAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
        T CurrentStatus { get; }
    }
}