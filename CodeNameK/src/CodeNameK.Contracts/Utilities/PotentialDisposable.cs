using System;

namespace CodeNameK.Core.Utilities
{
    public sealed class PotentialDisposable<T> : IDisposable where T : class
    {
        public T Value { get; }
        public PotentialDisposable(T instance)
        {
            Value = instance ?? throw new ArgumentNullException(nameof(instance));
        }
        public void Dispose()
        {
            if (Value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}