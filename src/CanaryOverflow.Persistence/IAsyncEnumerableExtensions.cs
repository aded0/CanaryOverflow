namespace CanaryOverflow.Persistence;

// ReSharper disable once InconsistentNaming
public static class IAsyncEnumerableExtensions
{
    public static IAsyncEnumerable<T> NotNull<T>(this IAsyncEnumerable<T?> enumerable) where T : class
    {
        return enumerable.Where(e => e is not null).Select(e => e!);
    }
}
