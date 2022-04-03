#nullable enable

using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CodeNameK.Droid
{
    internal class CategoryViewHolder : RecyclerView.ViewHolder
    {
        private readonly ILogger _logger;
        private readonly Action<int>? _clickCallback;

        public TextView? CategoryCaption { get; set; }

        public CategoryViewHolder(
            View itemView,
            ILogger<CategoryViewHolder> logger,
            Action<int>? clickCallback = null)
            : base(itemView)
        {
            CategoryCaption = itemView.FindViewById<TextView>(Resource.Id.CategoryItemCaption);
            itemView.Click += OnItemClick;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clickCallback = clickCallback;
        }

        private void OnItemClick(object sender, EventArgs e)
        {
            _logger.LogDebug(nameof(OnItemClick));
            int originalPosition = LayoutPosition;
            _clickCallback?.Invoke(LayoutPosition);
        }
    }
}