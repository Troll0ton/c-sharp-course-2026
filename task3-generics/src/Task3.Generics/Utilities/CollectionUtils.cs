namespace Task3.Generics.Utilities;

public static class CollectionUtils
{
    public static List<T> Distinct<T>(List<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var seen = new HashSet<T>();
        var result = new List<T>();

        foreach (var item in source)
        {
            if (seen.Add(item))
            {
                result.Add(item);
            }
        }

        return result;
    }

    public static Dictionary<TKey, List<TValue>> GroupBy<TValue, TKey>(
        List<TValue> source,
        Func<TValue, TKey> keySelector)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        var result = new Dictionary<TKey, List<TValue>>();

        foreach (var item in source)
        {
            var key = keySelector(item);

            if (!result.TryGetValue(key, out var group))
            {
                group = [];
                result.Add(key, group);
            }

            group.Add(item);
        }

        return result;
    }

    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
        Dictionary<TKey, TValue> first,
        Dictionary<TKey, TValue> second,
        Func<TValue, TValue, TValue> conflictResolver)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(conflictResolver);

        var result = new Dictionary<TKey, TValue>();

        foreach (var pair in first)
        {
            result.Add(pair.Key, pair.Value);
        }

        foreach (var pair in second)
        {
            if (result.TryGetValue(pair.Key, out var existingValue))
            {
                result[pair.Key] = conflictResolver(existingValue, pair.Value);
                continue;
            }

            result.Add(pair.Key, pair.Value);
        }

        return result;
    }

    public static T MaxBy<T, TKey>(List<T> source, Func<T, TKey> selector)
        where TKey : IComparable<TKey>
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        if (source.Count == 0)
        {
            throw new InvalidOperationException("Cannot find maximum in an empty collection.");
        }

        var maxItem = source[0];
        var maxKey = selector(maxItem);

        for (var index = 1; index < source.Count; index++)
        {
            var item = source[index];
            var key = selector(item);

            if (key.CompareTo(maxKey) > 0)
            {
                maxItem = item;
                maxKey = key;
            }
        }

        return maxItem;
    }
}
