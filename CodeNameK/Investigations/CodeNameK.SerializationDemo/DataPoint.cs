namespace CodeNameK.Serializations
{
    public record DataPoint
    {
        public Guid Id { get; init; }
        public DateTime WhenUTC { get; init; }
        public double Value { get; init; }
    }
}