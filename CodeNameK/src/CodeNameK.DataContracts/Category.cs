using System.Collections.Generic;

namespace CodeNameK.DataContracts
{
    public class Category
    {
        public string Id { get; set; }

        public IEnumerable<DataPoint> Values { get; set; }
    }
}