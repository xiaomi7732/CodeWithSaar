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
        private readonly Action<int, int>? _swapper;

        public TextView? CategoryCaption { get; set; }

        public CategoryViewHolder(
            View itemView,
            ILogger<CategoryViewHolder> logger,
            Action<int>? clickCallback = null,
            Action<int, int>? swapper = null) : base(itemView)
        {
            CategoryCaption = itemView.FindViewById<TextView>(Resource.Id.CategoryItemCaption);
            itemView.Click += OnItemClick;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clickCallback = clickCallback;
            _swapper = swapper;
        }

        private async void OnItemClick(object sender, EventArgs e)
        {
            try
            {
                int originalPosition = LayoutPosition;
                _swapper?.Invoke(LayoutPosition, 0);
                await Task.Delay(TimeSpan.FromMilliseconds(250)); // Delay for the animation to happen;
                _clickCallback?.Invoke(LayoutPosition);
                _swapper?.Invoke(0, originalPosition);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error.");
            }
        }
    }
}