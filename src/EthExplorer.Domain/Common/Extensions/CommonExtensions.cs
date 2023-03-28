namespace EthExplorer.Domain.Common.Extensions;

public static class CommonExtensions
{
    public static bool In<T>(this T val, params T[] values) where T : struct =>
        values.Contains(val);

    public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str?.Trim());

    public static bool IsDefault<T>(this T val) => EqualityComparer<T>.Default.Equals(val, default);
    
    public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> dic, KeyValuePair<TKey, TValue> pair)
    {
        dic.Add(pair.Key, pair.Value);
    }

    public static ulong ToUnixTimestamp(this DateTime dt) => (ulong)dt.Subtract(DateTime.UnixEpoch).TotalSeconds;
    
    public static DateTime FromUnixTimestamp(this ulong timestamp) => DateTime.SpecifyKind(DateTime.UnixEpoch.AddSeconds(timestamp), DateTimeKind.Utc);

    public static int Id(this Enum @enum) => Convert.ToInt32(@enum);
    
    public static bool HasItems<T>(this IEnumerable<T> source) => source?.Any() ?? false;
    
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (T element in source) action(element);
    }
}