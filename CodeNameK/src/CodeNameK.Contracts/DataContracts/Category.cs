using System.Collections.Generic;
using System.Linq;

namespace CodeNameK.DataContracts
{
    public record Category
    {
        public string? Id { get; init; }

        public IEnumerable<DataPoint> Values { get; init; } = Enumerable.Empty<DataPoint>();
    }
}