using Task3.Generics.Abstractions;

namespace Task3.Generics.Repositories;

public sealed class Repository<T>
    where T : IEntity
{
    private readonly Dictionary<int, T> _items = [];

    public int Count => _items.Count;

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (_items.ContainsKey(item.Id))
        {
            throw new InvalidOperationException($"Item with Id {item.Id} already exists.");
        }

        _items.Add(item.Id, item);
    }

    public bool Remove(int id)
    {
        return _items.Remove(id);
    }

    public T? GetById(int id)
    {
        return _items.TryGetValue(id, out var item) ? item : default;
    }

    public IReadOnlyList<T> GetAll()
    {
        return [.. _items.Values];
    }

    public IReadOnlyList<T> Find(Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var result = new List<T>();

        foreach (var item in _items.Values)
        {
            if (predicate(item))
            {
                result.Add(item);
            }
        }

        return result;
    }
}
