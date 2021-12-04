using System;
using CodeNameK.BIZ.Interfaces;
namespace CodeNameK.BIZ
{
    internal class BizChartAxisExpansion : IChartAxisExpansion
    {
        public double ExpandDown(double max)
        {
            return max - GetMargin(max);
        }

        public double ExpandUp(double max)
        {
            return max + GetMargin(max);
        }

        private double GetMargin(double value)
        {
            return Math.Abs(value) * 0.1;
        }
    }
}
