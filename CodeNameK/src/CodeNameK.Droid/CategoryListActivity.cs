#nullable enable

using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using CodeNameK.Droid.ViewModels;
using Google.Android.Material.FloatingActionButton;
using Microsoft.Extensions.Logging;
using System;

namespace CodeNameK.Droid
{
    [Activity(Label = "@string/category_list_activity_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class CategoryListActivity : KActivityBase, IAddCategoryDialogListener
    {
        private CategoryListAdapter? _adapter;
        private RecyclerView? _recyclerView;
        private FloatingActionButton? _fab;
        private ILogger? _logger;
        private CategoryListViewModel? _categoryListViewModel;

        protected override void OnCreating(Bundle? savedInstanceState)
        {
            // Create logger
            _logger = GetRequiredService<ILogger<CategoryListActivity>>();

            // Set our view from the "categorylist" layout resource:
            SetContentView(Resource.Layout.ActivityCategory);

            // FAB
            _fab = FindViewById<FloatingActionButton>(Resource.Id.fab);

            // Get our RecyclerView layout:
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.CategoryListRecyclerView) ?? throw new ArgumentException("Not able to find view by id. ", nameof(Resource.Id.CategoryListRecyclerView));
            // Plug in the linear layout manager:
            _recyclerView.SetLayoutManager(new LinearLayoutManager(this));


            // Setup viewmodel
            _categoryListViewModel = new ViewModelProvider(this).Get(Java.Lang.Class.FromType(typeof(CategoryListViewModel))) as CategoryListViewModel;
            if (savedInstanceState is not null)
            {
                // Restore
                _logger.LogInformation("Resotre in OnCreate");
            }
            else
            {
                _logger.LogInformation("First time in OnCreate");
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Instantiate the adapter and pass in its data source:
            _adapter = new CategoryListAdapter(
                _categoryListViewModel!.Categories,
                GetRequiredService<ILoggerFactory>());

            // Plug the adapter into the RecyclerView:
            _recyclerView!.SetAdapter(_adapter);
        }

        protected override void OnCreated(Bundle? savedInstanceState)
        {
            SupportActionBar?.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar?.SetDisplayShowHomeEnabled(true);

            base.OnCreated(savedInstanceState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            _adapter!.OnItemClicked += CategoryItemClicked;
            _fab!.Click += OnFabClicked;
        }

        private void OnFabClicked(object sender, EventArgs e)
        {
            AddCategoryDialogFragment addCategoryDialog = new AddCategoryDialogFragment();
            addCategoryDialog.Show(SupportFragmentManager, nameof(AddCategoryDialogFragment));
        }

        protected override void OnPause()
        {
            base.OnPause();
            _adapter!.OnItemClicked -= CategoryItemClicked;
            _fab!.Click -= OnFabClicked;
        }

        private void CategoryItemClicked(int index)
        {
            try
            {
                _logger?.LogInformation("Category {index} is clicked. Id: {categoryId}", index, _categoryListViewModel?.Categories?[index].Id);
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed clicking category item.");
            }
        }

        void IAddCategoryDialogListener.OnOKClicked(string newValue)
        {
            _logger!.LogInformation("OK button is clicked! New value: {newValue}", newValue);
        }
    }
}