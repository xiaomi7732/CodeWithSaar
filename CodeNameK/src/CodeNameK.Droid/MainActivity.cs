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
using CodeNameK.DataContracts;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using AndroidEnvironment = Android.OS.Environment;
using CodeNameK.Core.Utilities;
using System.Linq;

namespace CodeNameK.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private IServiceCollection? _services;
        private ServiceProvider? _serviceProvider;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _services = new ServiceCollection();
            _services = RegisterServices(_services);

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
            View view = (View)sender;

            ILocalPathProvider localPathProvider = ServiceProvider.GetRequiredService<ILocalPathProvider>();
            ICategoryRepo categoryRepo = ServiceProvider.GetRequiredService<ICategoryRepo>();
            string categoryId = "What can go wrong?";

            Category? category = categoryRepo.GetAllCategories().FirstOrDefault(c => string.Equals(c.Id, categoryId));
            category ??= new Category() { Id = categoryId };
            categoryRepo.AddCategoryAsync(category, cancellationToken: default).FireWithExceptionHandler(onException =>
            {
                // Do somehting.
                Snackbar.Make(view, $"Something went wrong: {onException}", Snackbar.LengthLong);
            });

            IDataPointRepo dataPointRepo = ServiceProvider.GetRequiredService<IDataPointRepo>();
            dataPointRepo.AddPointAsync(new DataPoint() { 
                Id = Guid.NewGuid(),
                Category = category,
                Value = (new Random()).Next(25000),
                WhenUTC = DateTime.UtcNow,
            }, cancellationToken: default);

            Snackbar.Make(view, $"New cateogry {categoryId} added.", Snackbar.LengthLong)
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
            services.AddLogging();
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
                // Notice: The device file explorer with Android stuido can't show hidden folder. So, "codeNameK/Data" is used 
                // instead of ".codeNameK/Data"
                localStoreOption.DataStorePath = Path.Combine(basePath, "codeNameK/Data");
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

        private IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider is null)
                {
                    if (_services is null)
                    {
                        throw new InvalidOperationException("Service Collection hasn't been created.");
                    }
                    _serviceProvider = _services.BuildServiceProvider();
                }

                return _serviceProvider;
            }
        }
    }
}
