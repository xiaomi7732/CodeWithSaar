#nullable enable

using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DataContracts;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Microsoft.Extensions.DependencyInjection;
using System;
using CodeNameK.Core.Utilities;
using System.Linq;

namespace CodeNameK.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private IServiceProvider _serviceProvider;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _serviceProvider = ((App?)Application)?.ServiceProvider ?? throw new InvalidProgramException("Service Provider doesn't exist.");
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
            IServiceProvider serviceProvider = _serviceProvider;

            ILocalPathProvider localPathProvider = serviceProvider.GetRequiredService<ILocalPathProvider>();
            ICategoryRepo categoryRepo = serviceProvider.GetRequiredService<ICategoryRepo>();
            string categoryId = "What can go wrong?";

            Category? category = categoryRepo.GetAllCategories().FirstOrDefault(c => string.Equals(c.Id, categoryId));
            category ??= new Category() { Id = categoryId };
            categoryRepo.AddCategoryAsync(category, cancellationToken: default).FireWithExceptionHandler(onException =>
            {
                // Do somehting.
                Snackbar.Make(view, $"Something went wrong: {onException}", Snackbar.LengthLong);
            });

            IDataPointRepo dataPointRepo = serviceProvider.GetRequiredService<IDataPointRepo>();
            dataPointRepo.AddPointAsync(new DataPoint()
            {
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
    }
}
