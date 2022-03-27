#nullable enable

using Android.App;
using Android.OS;
using AndroidX.RecyclerView.Widget;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.DataContracts;
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
        private RecyclerView _recyclerView;
        private RecyclerView.LayoutManager _layoutManager;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Prepare the data source
            ICategory categoryBiz = GetRequiredService<ICategory>();
            _categories = categoryBiz.GetAllCategories().ToList();
#if DEBUG
            _categories.Add(new Category()
            {
                Id = "Test Category 1",
            });

            _categories.Add(new Category()
            {
                Id = "Test Category 2",
            });
#endif

            // Instantiate the adapter and pass in its data source:
            _adapter = new CategoryListAdapter(_categories);

            // Set our view from the "categorylist" layout resource:
            SetContentView(Resource.Layout.CategoryList);

            // Get our RecyclerView layout:
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.CategoryListRecyclerView) ?? throw new ArgumentException("Not able to find view by id. ", nameof(Resource.Id.CategoryListRecyclerView));

            // Plug in the linear layout manager:
            _layoutManager = new LinearLayoutManager(this);
            _recyclerView.SetLayoutManager(_layoutManager);

            // Plug the adapter into the RecyclerView:
            _recyclerView?.SetAdapter(_adapter);
        }
    }
}