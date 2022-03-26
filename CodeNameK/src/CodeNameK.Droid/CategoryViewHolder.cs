#nullable enable

using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace CodeNameK.Droid
{
    internal class CategoryViewHolder : RecyclerView.ViewHolder
    {
        public TextView? CategoryCaption { get; set; }

        public CategoryViewHolder(View itemView) : base(itemView)
        {
            CategoryCaption = itemView.FindViewById<TextView>(Resource.Id.CategoryItemCaption);
        }
    }
}