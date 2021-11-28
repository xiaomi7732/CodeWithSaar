using System;
using System.Text.Json;
using BenchmarkDotNet.Attributes;

namespace CodeNameK.Benchmark.Serializations
{
    public class SerializationBenchmark
    {
        private static readonly string _manualSerializedData = "a76824ba-3c47-4056-9a74-5d98eeaa4413|2021-10-28T04:45:28.7034932Z|256";
        private static readonly string _jsonSerializedData = "{\"Id\":\"bda821e1-596c-4e35-a77d-dd793b42c1be\",\"OccurDateTime\":\"2021-10-28T04:50:13.0024993Z\",\"Value\":256}";

        private static readonly DataPoint _data = new DataPoint()
        {
            Id = Guid.NewGuid(),
            WhenUTC = DateTime.UtcNow,
            Value = 2000,
        };
        [Benchmark]
        public bool ManualSerialize()
        {
            string result = $"{_data.Id:N}|{_data.WhenUTC:O}|{_data.Value}";
            return !string.IsNullOrEmpty(result);
        }
        [Benchmark]
        public bool UseJsonSerialize()
        {
            string result = JsonSerializer.Serialize(_data);
            return !string.IsNullOrEmpty(result);
        }

        [Benchmark]
        public bool ManualDeserialize()
        {
            string[] tokens = _manualSerializedData.Split('|');
            DataPoint data = new DataPoint()
            {
                Id = Guid.Parse(tokens[0]),
                WhenUTC = DateTime.Parse(tokens[1]),
                Value = double.Parse(tokens[2]),
            };
            return data != null;
        }

        [Benchmark]
        public bool UseJsonDeserializerDeserialize()
        {
            DataPoint data = JsonSerializer.Deserialize<DataPoint>(_jsonSerializedData);
            return data != null;
        }
    }
}