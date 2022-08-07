#nullable enable

using Android.Views;
using AndroidX.RecyclerView.Widget;
using CodeNameK.DataContracts;
using System;
using System.Collections.Generic;

namespace CodeNameK.Droid
{
    internal class NumbersAdapter : RecyclerView.Adapter
    {
        private readonly IReadOnlyList<DataPoint> _dataPoints;

        public NumbersAdapter(IReadOnlyList<DataPoint> dataPoints)
        {
            _dataPoints = dataPoints ?? throw new System.ArgumentNullException(nameof(dataPoints));
        }

        public override int ItemCount => _dataPoints.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is NumbersViewHolder numberViewHolder)
            {
                DataPoint dataPoint = _dataPoints[position];
                numberViewHolder.TextViewTimestamp.Text = dataPoint.WhenUTC.ToLocalTime().ToString("o");
                numberViewHolder.TextViewNumber.Text = dataPoint.Value.ToString("N2");
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context)?.Inflate(Resource.Layout.item_number, parent, attachToRoot: false)
                ?? throw new InvalidOperationException("Can't inflate Resource.Layout.item_number");
            NumbersViewHolder numbersViewHolder = new NumbersViewHolder(itemView);
            return numbersViewHolder;
        }
    }
}