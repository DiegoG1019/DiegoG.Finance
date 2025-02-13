using System.Collections;

namespace DiegoG.Finance.Internal;

internal sealed class CastCollectionWrappers<TTo, TFrom>(IReadOnlyCollection<TFrom> collection) : IReadOnlyCollection<TTo>
    where TFrom : class
    where TTo : TFrom
{
    public int Count => collection.Count;

    public IEnumerator<TTo> GetEnumerator()
    {
        foreach (var item in collection) 
            yield return (TTo)item;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
