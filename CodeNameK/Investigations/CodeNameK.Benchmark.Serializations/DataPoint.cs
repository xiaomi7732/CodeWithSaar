using System;

namespace CodeNameK.Benchmark.Serializations
{
    public class DataPoint
    {
        public Guid Id { get; set; }
        public DateTime WhenUTC { get; set; }
        public double Value { get; set; }
    }
}