#nullable enable

using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.Widget;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.Core.Utilities;
using CodeNameK.DataContracts;
using CodeNameK.Droid.ViewModels;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Microsoft.Extensions.Logging;
using System;

namespace CodeNameK.Droid
{
    [Activity(Label = "@string/category_list_activity_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class CategoryListActivity : KActivityBase, IAddCategoryDialogEventListener
    {
        private CategoryListAdapter? _adapter;
        private RecyclerView? _recyclerView;
        private FloatingActionButton? _fab;
        private CategoryListViewModel? _categoryListViewModel;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "categorylist" layout resource:
            SetContentView(Resource.Layout.activity_category);

            // Setup toolbar
            Toolbar? toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar?.SetDisplayHomeAsUpEnabled(false);
            SupportActionBar?.SetDisplayShowHomeEnabled(true);

            // FAB
            _fab = FindViewById<FloatingActionButton>(Resource.Id.fab);

            // Get our RecyclerView layout:
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.category_list_recycler_view) ?? throw new ArgumentException("Not able to find view by id. ", nameof(Resource.Id.category_list_recycler_view));
            // Plug in the linear layout manager:
            _recyclerView.SetLayoutManager(new LinearLayoutManager(this));

            // Setup viewmodel
            _categoryListViewModel = new ViewModelProvider(this).Get(Java.Lang.Class.FromType(typeof(CategoryListViewModel))) as CategoryListViewModel;
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

        protected override void OnResume()
        {
            base.OnResume();
            _adapter!.OnItemClicked += CategoryItemClicked;
            _fab!.Click += OnFabClicked;
        }

        private void OnFabClicked(object sender, EventArgs e)
        {
            AndroidX.Fragment.App.DialogFragment addCategoryDialog = new AddCategoryDialogFragment();
            addCategoryDialog.Show(SupportFragmentManager, "AddCategoryDialog");
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
                Logger.LogInformation("Category {index} is clicked. Id: {categoryId}", index, _categoryListViewModel?.Categories?[index].Id);
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed clicking category item.");
            }
        }

        /// <summary>
        /// Invokes when ok button clicked on add category dialog.
        /// </summary>
        /// <param name="category"></param>
        void IAddCategoryDialogEventListener.OnOKClicked(string category)
        {
            Logger.LogInformation("Adding category clicked. Content: {value}", category);
            ICategory categoryBiz = GetRequiredService<ICategory>();
            categoryBiz.AddCategoryAsync(new Category() { Id = category }).FireWithHandlers<OperationResult<Category>>(onSuccess: (result) =>
            {
                if (result is not null && result.IsSuccess)
                {
                    Snackbar.Make(_fab, $"New category: {category}", Snackbar.LengthLong).Show();
                }
                else
                {
                    Snackbar.Make(_fab, $"Failed: {result?.Reason}", Snackbar.LengthLong).Show();
                }
            }, onException: ShowExceptionMessage, continueOnSychronizationContext: false);
        }

        /// <summary>
        /// Invokes when cancel button clicked on add category dialog.
        /// </summary>
        /// <param name="category"></param>
        void IAddCategoryDialogEventListener.OnCancelClicked(string category)
        {
            Logger.LogInformation("Cancel adding category clicked. Content: {value}", category);
        }

        /// <summary>
        /// Shows exceptions.
        /// </summary>
        private void ShowExceptionMessage(Exception ex)
        {
            Snackbar.Make(_fab, $"Error: {ex.Message}", Snackbar.LengthIndefinite).Show();
        }

        protected override ILogger GetLoggerInstance()
        {
            return GetRequiredService<ILogger<CategoryListActivity>>();
        }
    }
}