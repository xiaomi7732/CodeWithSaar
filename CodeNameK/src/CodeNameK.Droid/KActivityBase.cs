#nullable enable

using Android.OS;
using AndroidX.AppCompat.App;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace CodeNameK.Droid
{
    /// <summary>
    /// A thin wrapper for AppCompatActivity.
    /// Primary provides the service provider and the logger.
    /// </summary>
    public abstract class KActivityBase : AppCompatActivity
    {
        private IServiceProvider? _serviceProvider = null;
        private IServiceScope? _serviceScope = null;

        /// <summary>
        /// Activities upon creating the view.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        /// <exception cref="InvalidProgramException"></exception>
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _serviceProvider = ((App?)Application)?.ServiceProvider ?? throw new InvalidProgramException("Service Provider doesn't exist.");
            _serviceScope = _serviceProvider.CreateScope();
            Logger.LogInformation("Lifetime Method: {name}, is savedInstanceState null: {isSavedInstanceStateNull}", nameof(OnCreate), savedInstanceState is null);
        }

        protected override void OnStart()
        {
            base.OnStart();
            Logger.LogInformation("Lifetime Method: {name}", nameof(OnStart));
        }

        protected override void OnResume()
        {
            base.OnResume();
            Logger.LogInformation("Lifetime Method: {name}", nameof(OnResume));
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            Logger.LogInformation("Lifetime Method: {name}", nameof(OnRestoreInstanceState));
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            Logger.LogInformation("Lifetime Method: {name}", nameof(OnSaveInstanceState));
            base.OnSaveInstanceState(outState);
        }

        protected override void OnPause()
        {
            Logger.LogInformation("Lifetime Method: {name}", nameof(OnPause));
            base.OnPause();
        }

        protected override void OnStop()
        {
            Logger.LogInformation("Lifetime Method: {name}", nameof(OnStop));
            _serviceScope?.Dispose();
            _serviceScope = null;
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Logger.LogInformation("Lifetime Method: {name}, Is finishing: {isFinishing}", nameof(OnDestroy), IsFinishing);
            base.OnDestroy();
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
            if (_serviceScope is null)
            {
                throw new InvalidOperationException("Service provider hasn't been initialized. Have you called OnCreate yet?");
            }
            return _serviceScope.ServiceProvider.GetRequiredService<T>();
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

                _serviceScope?.Dispose();
                _serviceScope = null;
            }
        }

        /// <summary>
        /// Gets or the logger instance for current class.
        /// </summary>
        protected ILogger Logger
        {
            get
            {
                return GetLoggerInstance();
            }
        }

        /// <summary>
        /// Gets logger instance from logging providers.
        /// </summary>
        private ILogger GetLoggerInstance()
        {
            ILoggerFactory loggerFactory = _serviceProvider!.GetRequiredService<ILoggerFactory>();
            return loggerFactory.CreateLogger(GetType().FullName);
        }
    }
}