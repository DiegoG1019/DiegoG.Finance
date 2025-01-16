using MessagePack;
using NodaMoney;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace DiegoG.Finance;

public abstract class CategorizedFinancialCollection<T> : IReadOnlyDictionary<string, T>
{
    internal CategorizedFinancialCollection(Dictionary<string, T> categories)
    {
        _categories = categories ?? [];
    }

    public CategorizedFinancialCollection(int capacity)
    {
        _categories = new(capacity);
    }

    public CategorizedFinancialCollection()
    {
        _categories = [];
    }

    internal protected readonly Dictionary<string, T> _categories;

    public T this[string key] => _categories[key];

    public int Count => _categories.Count;

    public ICollection<string> Keys => _categories.Keys;

    public ICollection<T> Values => _categories.Values;

    IEnumerable<string> IReadOnlyDictionary<string, T>.Keys => _categories.Keys;

    IEnumerable<T> IReadOnlyDictionary<string, T>.Values => _categories.Values;

    public event Action<CategorizedFinancialCollection<T>, NotifyCollectionChangedAction, KeyValuePair<string, T>?>? CollectionChanged;

    public virtual T Add(string key)
    {
        var val = ValueFactory(key);
        _categories.Add(key, val);
        CollectionChanged?.Invoke(this, NotifyCollectionChangedAction.Add, new(key, val));
        return val;
    }

    public virtual void Clear()
    {
        _categories.Clear();
        CollectionChanged?.Invoke(this, NotifyCollectionChangedAction.Reset, null);
    }

    public bool ContainsKey(string key) => _categories.ContainsKey(key);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _categories.GetEnumerator();
    }

    public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
    {
        return _categories.GetEnumerator();
    }

    public virtual bool Remove(string key, [MaybeNullWhen(false)] out T value)
    {
        if (_categories.Remove(key, out value))
        {
            CollectionChanged?.Invoke(this, NotifyCollectionChangedAction.Remove, new(key, value));
            return true;
        }
        return false;
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out T value) => _categories.TryGetValue(key, out value);

    protected abstract T ValueFactory(string key);
}