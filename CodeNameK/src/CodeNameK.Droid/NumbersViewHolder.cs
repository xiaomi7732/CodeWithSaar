#nullable enable

using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;

namespace CodeNameK.Droid
{
    internal class NumbersViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextViewTimestamp { get; }
        public TextView TextViewNumber { get; }

        public NumbersViewHolder(View itemView) : base(itemView)
        {
            TextViewTimestamp = itemView.FindViewById<TextView>(Resource.Id.text_view_timestamp) ?? throw new InvalidOperationException("Timestamp textview is not found.");
            TextViewNumber = itemView.FindViewById<TextView>(Resource.Id.text_view_value) ?? throw new InvalidOperationException("Value text view is not found.");
        }
    }
}