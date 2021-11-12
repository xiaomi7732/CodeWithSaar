using System.Collections.Generic;

namespace CodeNameK.DataContracts
{
    public record Category
    {
        public string Id { get; init; }

        public IEnumerable<DataPoint> Values { get; init; }
    }
}