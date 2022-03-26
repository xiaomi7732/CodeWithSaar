#nullable enable

using Android.App;
using Android.Runtime;
using CodeNameK.BIZ;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.DAL;
using CodeWithSaar.Extensions.Logging.Android;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using AndroidEnvironment = Android.OS.Environment;

namespace CodeNameK.Droid;

[Application]
public class App : Application
{
    private ServiceProvider? _serviceProvider;
    public IServiceProvider ServiceProvider => _serviceProvider ?? throw new ObjectDisposedException(nameof(ServiceProvider));

    protected App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {

    }

    public override void OnCreate()
    {
        Initialize();
        base.OnCreate();
    }

    private void Initialize()
    {
        ServiceCollection serviceCollection = new ServiceCollection();
        AddServices(serviceCollection);

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddLogging(loggerBuilder =>
        {
            loggerBuilder.AddAndroid();
        });

        services.AddOptions<LocalStoreOptions>().Configure(localStoreOption =>
        {
            bool isReadonly = AndroidEnvironment.MediaMountedReadOnly.Equals(AndroidEnvironment.ExternalStorageState);
            bool isWriteable = AndroidEnvironment.MediaMounted.Equals(AndroidEnvironment.ExternalStorageState);

            if (isReadonly || !isWriteable)
            {
                throw new InvalidOperationException("No good storage.");
            }

            string? basePath = Context.GetExternalFilesDir(null)?.AbsolutePath;
            if (string.IsNullOrEmpty(basePath))
            {
                throw new InvalidOperationException("No valid storage for the app.");
            }
            // Notice: The device file explorer with Android stuido can't show hidden folder. So, "codeNameK/Data" is used 
            // instead of ".codeNameK/Data"
            localStoreOption.DataStorePath = Path.Combine(basePath, "codeNameK/Data");
        });
#pragma warning disable CS0618 // For testing purpose
        services.RegisterDataAccessModuleForAndroid();
        services.RegisterBizModuleForAndroid();
#pragma warning restore CS0618 // Type or member is obsolete
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _serviceProvider?.Dispose();
            _serviceProvider = null;
        }
    }
}