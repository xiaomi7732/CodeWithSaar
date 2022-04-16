#nullable enable

using Android.App;
using Android.OS;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.FloatingActionButton;
using Microsoft.Extensions.Logging;
using System;

namespace CodeNameK.Droid
{
    [Activity(Theme = "@style/AppTheme.NoActionBar", MainLauncher = false, ParentActivity = typeof(CategoryListActivity))]
    public class NumbersActivity : KActivityBase
    {
        private string? _categoryName;
        private FloatingActionButton? _fab;
        private RecyclerView? _recyclerView;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_numbers);

            if (savedInstanceState is null)
            {
                string key = this.MakeIntentKeyForApp(GetString(Resource.String.key_category_name));
                Logger.LogDebug("Fetch string content by key: {key}. Is intend null? => {value}", key, Intent is null);
                _categoryName = Intent?.GetStringExtra(key);
                Logger.LogDebug("Get _categoryName: {value}", _categoryName);
            }
            else
            {
                _categoryName = savedInstanceState.GetString(GetString(Resource.String.key_category_name));
            }

            if (string.IsNullOrEmpty(_categoryName))
            {
                Logger.LogError("Category name is null or empty.");
                throw new InvalidOperationException("Category name is null or empty");
            }
            Logger.LogInformation("Category Name: {value}", _categoryName);

            // Setup toolbar
            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar) ?? throw new InvalidCastException("Can't find a tool bar.");
            SetSupportActionBar(toolbar);
            SupportActionBar?.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar?.SetDisplayShowHomeEnabled(true);
            SupportActionBar!.Title = _categoryName;

            // FAB
            _fab = FindViewById<FloatingActionButton>(Resource.Id.fab);

            // Get our RecyclerView layout:
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.category_list_recycler_view) ?? throw new ArgumentException("Not able to find view by id. ", nameof(Resource.Id.category_list_recycler_view));
            // Plug in the linear layout manager:
            _recyclerView.SetLayoutManager(new LinearLayoutManager(this));

            // Setup viewmodel
            // _categoryListViewModel = new ViewModelProvider(this).Get(Java.Lang.Class.FromType(typeof(CategoryListViewModel))) as CategoryListViewModel;
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString(GetString(Resource.String.key_category_name), _categoryName);
            base.OnSaveInstanceState(outState);
        }
    }
}