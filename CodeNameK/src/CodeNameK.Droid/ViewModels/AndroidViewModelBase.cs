#nullable enable

using Android.App;
using AndroidX.Lifecycle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CodeNameK.Droid.ViewModels
{
    internal class AndroidViewModelBase : AndroidViewModel, INotifyPropertyChanged
    {
        private readonly App _application;
        private readonly ILogger _logger;
        public AndroidViewModelBase(Application application) : base(application)
        {
            if (application is not App app)
            {
                throw new InvalidCastException("Can't cast Application instance to App instance.");
            }
            _application = app;
            _logger = GetRequiredService<ILogger<AndroidViewModelBase>>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected T GetRequiredService<T>() where T : notnull
            => _application.ServiceProvider.GetRequiredService<T>();

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            _logger.LogInformation("Raising property changed: {propertyName}", propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}