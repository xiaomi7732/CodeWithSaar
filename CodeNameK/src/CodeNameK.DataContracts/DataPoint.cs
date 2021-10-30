using System;
using System.Text.Json.Serialization;

namespace CodeNameK.DataContracts
{
    public class DataPoint
    {
        public Guid Id { get; set; }
        public DateTime WhenUTC { get; set; }
        public double Value { get; set; }
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public Category Category { get; set; }
    }
}