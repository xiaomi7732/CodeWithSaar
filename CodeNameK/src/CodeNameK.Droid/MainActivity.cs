#nullable enable

using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.DAL;
using CodeNameK.DAL.Interfaces;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using AndroidEnvironment = Android.OS.Environment;

namespace CodeNameK.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            IServiceCollection services = new ServiceCollection();
            services = RegisterServices(services);
            _serviceProvider = services.BuildServiceProvider();

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Toolbar? toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton? fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            if (fab is not null)
            {
                fab.Click += FabOnClick;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu? menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            ILocalPathProvider localPathProvider = _serviceProvider?.GetRequiredService<ILocalPathProvider>();
            string text = localPathProvider.BasePath;

            View view = (View)sender;
            Snackbar.Make(view, text, Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener?)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private IServiceCollection RegisterServices(IServiceCollection services)
        {
#pragma warning disable CS0618 // Type or member is obsolete - for testing only
            services.AddOptions<LocalStoreOptions>().Configure(localStoreOption =>
            {
                bool isReadonly = AndroidEnvironment.MediaMountedReadOnly.Equals(AndroidEnvironment.ExternalStorageState);
                bool isWriteable = AndroidEnvironment.MediaMounted.Equals(AndroidEnvironment.ExternalStorageState);

                if (isReadonly || !isWriteable)
                {
                    throw new InvalidOperationException("No good storage.");
                }
                
                string? basePath = ApplicationContext?.GetExternalFilesDir(null)?.AbsolutePath;
                if (string.IsNullOrEmpty(basePath))
                {
                    throw new InvalidOperationException("No valid storage for the app.");
                }
                localStoreOption.DataStorePath = Path.Combine(basePath, "codeNameK/Data");
                Directory.CreateDirectory(localStoreOption.DataStorePath);

            });
            services.RegisterDataAccessModuleForAndroid();
#pragma warning restore CS0618 // Type or member is obsolete
            return services;
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
}
