namespace CodeNameK.DataContracts
{
    public record DataPointPathInfo : DataPointInfo
    {
        public ushort YearFolder { get; init; }
        public ushort MonthFolder { get; init; }
        public bool IsDeletionMark { get; init; }
    }
}