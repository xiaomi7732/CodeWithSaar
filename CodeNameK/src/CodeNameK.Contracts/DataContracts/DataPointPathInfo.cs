using System.Collections.Generic;

namespace CodeNameK.DataContracts
{
    public record DataPointPathInfo : DataPointInfo, IEqualityComparer<DataPointPathInfo>
    {
        public ushort YearFolder { get; init; }
        public ushort MonthFolder { get; init; }
        public bool IsDeletionMark { get; init; }

        public bool Equals(DataPointPathInfo x, DataPointPathInfo y)
        {
            if (x is null || y is null || x.Category?.Id is null || y.Category?.Id is null)
            {
                return false;
            }

            // Same object
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            // Otherwise
            return x.Id.Equals(y.Id) &&
                x.Category.Id.Equals(y.Category.Id, System.StringComparison.OrdinalIgnoreCase) &&
                x.YearFolder == y.YearFolder &&
                x.MonthFolder == y.MonthFolder &&
                x.IsDeletionMark == y.IsDeletionMark;

        }

        public int GetHashCode(DataPointPathInfo obj)
        {
            if (obj is null)
            {
                return 0;
            }

            int hashCode = obj.Id.GetHashCode();
            if (!string.IsNullOrEmpty(obj.Category?.Id))
            {
                hashCode = hashCode ^ obj.Category.Id.GetHashCode();
            }
            if (YearFolder != 0)
            {
                hashCode = hashCode ^ YearFolder;
            }
            if (MonthFolder != 0)
            {
                hashCode = hashCode ^ MonthFolder;
            }
            hashCode = hashCode ^ IsDeletionMark.GetHashCode();

            return hashCode;
        }
    }
}