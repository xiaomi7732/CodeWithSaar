using System;
using System.Collections.Generic;

namespace CodeNameK.DataContracts
{
    public sealed class CategoryIdOrdinalIgnoreCaseComparer : Comparer<Category>
    {
        private CategoryIdOrdinalIgnoreCaseComparer()
        {
        }
        public static CategoryIdOrdinalIgnoreCaseComparer Instance { get; } = new CategoryIdOrdinalIgnoreCaseComparer();
        public override int Compare(Category x, Category y) => string.Compare(x?.Id, y?.Id, StringComparison.OrdinalIgnoreCase);
    }
}
