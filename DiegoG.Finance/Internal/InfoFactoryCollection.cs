using System.Collections;

namespace DiegoG.Finance.Internal;

internal class InfoFactoryCollection<TInfo, TValue>(IEnumerable<TInfo> enumer, Func<TInfo, TValue> factory) : IEnumerable<TValue>
{
    public IEnumerator<TValue> GetEnumerator()
    {
        foreach (var item in enumer)
            yield return factory(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
