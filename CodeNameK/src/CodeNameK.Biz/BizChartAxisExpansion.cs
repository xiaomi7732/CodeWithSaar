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

            //int sign = max >= 0 ? 1 : -1;
            //double absValue = Math.Abs(max);

            //double candidate1 = (Math.Log(absValue) + 1) * 10;// (Log(x) + 1) * 10
            //double candidate2 = absValue * 0.1; // 10%

            //double final = max + Math.Min(candidate1, candidate2);
            //return sign * final;
        }

        private double GetMargin(double value)
        {
            return Math.Abs(value) * 0.05; // 5%
        }
    }
}
