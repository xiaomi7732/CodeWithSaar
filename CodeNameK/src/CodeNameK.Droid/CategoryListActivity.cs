#nullable enable

using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.DataContracts;
using System.Collections.Generic;
using System.Linq;

namespace CodeNameK.Droid
{
    //[Activity(Label = "@string/app_name")]
    //[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]

    public class CategoryListActivity : KActivityBase
    {
        private List<Category>? _categories;
        private CategoryListAdapter? _adapter;
        private RecyclerView? _recyclerView;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Prepare the data source
            ICategory categoryBiz = GetRequiredService<ICategory>();
            _categories = categoryBiz.GetAllCategories().ToList();

            // Instantiate the adapter and pass in its data source:
            _adapter = new CategoryListAdapter(_categories);

            // Set our view from the "categorylist" layout resource:
            SetContentView(Resource.Layout.CategoryList);

            // Get our RecyclerView layout:
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.CategoryListRecyclerView);

            // Plug the adapter into the RecyclerView:
            _recyclerView?.SetAdapter(_adapter);
        }
    }
}