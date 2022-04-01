#nullable enable

using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.RecyclerView.Widget;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeNameK.Droid
{
    [Activity(Label = nameof(CategoryListActivity), MainLauncher = true)]
    public class CategoryListActivity : KActivityBase
    {
        private List<Category>? _categories;
        private CategoryListAdapter? _adapter;
        private RecyclerView? _recyclerView;
        private RecyclerView.LayoutManager? _layoutManager;
        private ILogger? _logger;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create logger
            _logger = GetRequiredService<ILogger<CategoryListActivity>>();

            // Prepare the data source
            ICategory categoryBiz = GetRequiredService<ICategory>();
            List<Category> categoryList = _categories = categoryBiz.GetAllCategories().ToList();
#if DEBUG
            for (int i = 0; i < 1000; i++)
            {
                _categories.Add(new Category()
                {
                    Id = "Test Category " + (i + 1),
                });
            }
#endif

            // Instantiate the adapter and pass in its data source:
            CategoryListAdapter adapter = _adapter = new CategoryListAdapter(
                categoryList,
                GetRequiredService<ILoggerFactory>());

            // Set our view from the "categorylist" layout resource:
            SetContentView(Resource.Layout.CategoryList);

            // Get our RecyclerView layout:
            RecyclerView recyclerView = FindViewById<RecyclerView>(Resource.Id.CategoryListRecyclerView) ?? throw new ArgumentException("Not able to find view by id. ", nameof(Resource.Id.CategoryListRecyclerView));
            _recyclerView = recyclerView;

            // Plug in the linear layout manager:
            RecyclerView.LayoutManager layoutManager = _layoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(layoutManager);

            // Plug the adapter into the RecyclerView:
            recyclerView.SetAdapter(adapter);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (_adapter is not null)
            {
                _adapter.OnItemClicked += CategoryItemClicked;
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (_adapter is not null)
            {
                _adapter.OnItemClicked -= CategoryItemClicked;
            }
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