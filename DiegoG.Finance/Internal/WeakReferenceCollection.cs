using System.Collections;

namespace DiegoG.Finance.Internal;

internal sealed class WeakReferenceSetCollection<T> : IReadOnlyCollection<T>
    where T : class
{
    private readonly HashSet<WeakReferenceStruct<T>> set = new(WeakReferenceStruct<T>.WeakReferenceStructEqualityComparer.Instance);

    public bool Add(T item)
        => set.Add(new(item));

    public void Clear()
    {
        foreach (var item in set)
            item.Dispose();
        set.Clear();
    }

    public bool Contains(T item)
    {
        var x = new WeakReferenceStruct<T>(item);
        try
        {
            return set.Contains(x);
        }
        finally
        {
            x.Dispose();
        }
    }

    public bool Remove(T item)
    {
        var x = new WeakReferenceStruct<T>(item);
        try
        {
            return set.Remove(x);
        }
        finally
        {
            x.Dispose();
        }
    }

    public int Count => set.Count;

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var x in set)
            if (x.TryGetTarget(out var t))
                yield return t;
            else
                set.Remove(x);
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
