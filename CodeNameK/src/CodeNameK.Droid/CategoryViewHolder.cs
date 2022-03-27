#nullable enable

using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;

namespace CodeNameK.Droid
{
    internal class CategoryViewHolder : RecyclerView.ViewHolder
    {
        public TextView? CategoryCaption { get; set; }

        public CategoryViewHolder(View itemView, Action<int>? clickCallback = null) : base(itemView)
        {
            CategoryCaption = itemView.FindViewById<TextView>(Resource.Id.CategoryItemCaption);
            itemView.Click += (sender, e) => clickCallback?.Invoke(LayoutPosition);
        }
    }
}