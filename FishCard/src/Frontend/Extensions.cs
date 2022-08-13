namespace CodeWithSaar.FishCard.Utilities;

internal static class Extensions
{
    public static IEnumerable<T> NullAsEmpty<T>(this IEnumerable<T>? input)
        => input ?? Enumerable.Empty<T>();
}