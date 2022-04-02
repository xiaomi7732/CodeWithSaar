#nullable enable

using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CodeNameK.Droid
{
    public abstract class KActivityBase : AppCompatActivity
    {
        private IServiceProvider? _serviceProvider = null;

        /// <summary>
        /// Activities upon creating the view.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        /// <exception cref="InvalidProgramException"></exception>
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            _serviceProvider = ((App?)Application)?.ServiceProvider ?? throw new InvalidProgramException("Service Provider doesn't exist.");
            OnCreating(savedInstanceState);
            base.OnCreate(savedInstanceState);
            OnCreated(savedInstanceState);
        }

        /// <summary>
        /// Activities to get the view ready before OnCreate.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected abstract void OnCreating(Bundle? savedInstanceState);

        /// <summary>
        /// Get the resource id for toolbar to set support action bar.
        /// </summary>
        /// <returns></returns>
        protected virtual int GetToolbarResId() => Resource.Id.toolbar;

        /// <summary>
        /// Activities post OnCreate.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected virtual void OnCreated(Bundle? savedInstanceState)
        {
            Toolbar? toolbar = FindViewById<Toolbar>(GetToolbarResId());
            SetSupportActionBar(toolbar);
        }

        /// <summary>
        /// Shorthands for the derived activity to get a service from the IoC container.
        /// </summary>
        /// <typeparam name="T">The service type to request.</typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        /// Throws when the service prvoider hasn't been initialized.
        /// </exception>
        protected T GetRequiredService<T>()
            where T : notnull
        {
            if (_serviceProvider is null)
            {
                throw new InvalidOperationException("Service provider hasn't been initialized. Have you called OnCreate yet?");
            }
            return _serviceProvider.GetRequiredService<T>();
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
    }
}