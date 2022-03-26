#nullable enable

using Android.OS;
using AndroidX.AppCompat.App;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CodeNameK.Droid
{
    public abstract class KActivityBase : AppCompatActivity
    {
        private IServiceProvider? _serviceProvider = null;
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _serviceProvider = ((App?)Application)?.ServiceProvider ?? throw new InvalidProgramException("Service Provider doesn't exist.");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (_serviceProvider is not null && _serviceProvider is IDisposable disposableServiceProvider)
                {
                    disposableServiceProvider.Dispose();
                    _serviceProvider = null;
                }
            }
        }

        protected T GetRequiredService<T>()
            where T : notnull
        {
            if (_serviceProvider is null)
            {
                throw new InvalidOperationException("Service provider hasn't been initialized. Have you called OnCreate yet?");
            }
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}