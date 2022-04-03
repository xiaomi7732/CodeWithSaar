#nullable enable

using Android.Views;
using AndroidX.RecyclerView.Widget;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace CodeNameK.Droid
{
    internal class CategoryListAdapter : RecyclerView.Adapter
    {
        private readonly IList<Category> _categories;
        private readonly ILoggerFactory _loggerFactory;

        public event Action<int>? OnItemClicked;

        public CategoryListAdapter(
            IList<Category> categories,
            ILoggerFactory loggerFactory)
        {
            _categories = categories ?? throw new ArgumentNullException(nameof(categories));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public override int ItemCount => _categories.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is CategoryViewHolder categoryViewHolder)
            {
                if (categoryViewHolder.CategoryCaption is null)
                {
                    throw new InvalidCastException("Category view holder shall always have a caption text view.");
                }
                categoryViewHolder.CategoryCaption.Text = _categories[position].Id;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the View for the category
            View? itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.item_category, parent, attachToRoot: false);
            if (itemView is null)
            {
                throw new InvalidOperationException("Failed inflating view for category");
            }
            CategoryViewHolder viewHolder = new CategoryViewHolder(
                itemView,
                _loggerFactory.CreateLogger<CategoryViewHolder>(),
                OnItemClicked);
            return viewHolder;
        }
    }
}