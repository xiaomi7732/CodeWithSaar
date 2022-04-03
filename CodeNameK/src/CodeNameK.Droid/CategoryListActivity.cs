#nullable enable

using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.RecyclerView.Widget;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.DataContracts;
using Google.Android.Material.FloatingActionButton;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeNameK.Droid
{
    [Activity(Label = "@string/category_list_activity_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class CategoryListActivity : KActivityBase
    {
        private List<Category>? _categories;
        private CategoryListAdapter? _adapter;
        private RecyclerView? _recyclerView;
        private RecyclerView.LayoutManager? _layoutManager;
        private FloatingActionButton? _fab;
        private ILogger? _logger;

        protected override void OnCreating(Bundle? savedInstanceState)
        {
            // Create logger
            _logger = GetRequiredService<ILogger<CategoryListActivity>>();

            // Prepare the data source
            ICategory categoryBiz = GetRequiredService<ICategory>();
            List<Category> categoryList = _categories = categoryBiz.GetAllCategories().ToList();
#if DEBUG
            if (categoryList.Count == 0)
            {
                for (int i = 0; i < 100; i++)
                {
                    _categories.Add(new Category()
                    {
                        Id = "Test Category " + (i + 1),
                    });
                }
            }
#endif

            // Instantiate the adapter and pass in its data source:
            CategoryListAdapter adapter = _adapter = new CategoryListAdapter(
                categoryList,
                GetRequiredService<ILoggerFactory>());

            // Set our view from the "categorylist" layout resource:
            SetContentView(Resource.Layout.ActivityCategory);

            // Get our RecyclerView layout:
            RecyclerView recyclerView = FindViewById<RecyclerView>(Resource.Id.CategoryListRecyclerView) ?? throw new ArgumentException("Not able to find view by id. ", nameof(Resource.Id.CategoryListRecyclerView));
            _recyclerView = recyclerView;

            // Plug in the linear layout manager:
            RecyclerView.LayoutManager layoutManager = _layoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(layoutManager);

            // Plug the adapter into the RecyclerView:
            recyclerView.SetAdapter(adapter);

            // FAB
            _fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
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
            AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this);
            AlertDialog? dialog = dialogBuilder.Create();
            if (dialog is null)
            {
                return;
            }
            dialog.SetTitle(GetString(Resource.String.category_list_add_category_title));
            dialog.SetMessage("Simple Alert");
            dialog.SetButton(GetString(Resource.String.ok), (sender, e) => { });
            dialog.SetButton2(GetString(Resource.String.cancel), (sender, e) => { });
            dialog.Show();
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
                _logger?.LogInformation("Category {index} is clicked. Id: {categoryId}", index, _categories?[index].Id);
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed clicking category item.");
            }
        }
    }
}