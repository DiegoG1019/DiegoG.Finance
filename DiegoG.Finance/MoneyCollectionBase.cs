using System.Collections.Specialized;
using System.Diagnostics;

namespace DiegoG.Finance;

public interface IFinancialEntry
{
    public decimal Amount { get; set; }
}

public abstract class MoneyCollectionBase<TCollection, TEntry> : MoneyCollectionBase<TCollection, TEntry, HashSet<TEntry>, TEntry>
    where TCollection : MoneyCollectionBase<TCollection, TEntry>
    where TEntry : IFinancialEntry
{
    internal MoneyCollectionBase(IEnumerable<TEntry>? amounts = null) : base(amounts) { }

    protected override TEntry GetEntry(TEntry entry) => entry;
    protected override TEntry GetInnerEntry(TEntry entry) => entry;
}

public abstract class MoneyCollectionBase<TCollection, TEntry, TInnerCollection, TInnerEntry>
    where TCollection : MoneyCollectionBase<TCollection, TEntry, TInnerCollection, TInnerEntry>
    where TEntry : IFinancialEntry
    where TInnerCollection : ICollection<TInnerEntry>
{
    internal protected readonly TInnerCollection _moneylist;

    protected abstract TInnerCollection InnerCollectionFactory(IEnumerable<TEntry>? amounts);
    protected abstract TEntry GetEntry(TInnerEntry entry);
    protected abstract TInnerEntry GetInnerEntry(TEntry entry);

    internal MoneyCollectionBase(IEnumerable<TEntry>? amounts = null)
    {
        _moneylist = InnerCollectionFactory(amounts);
        RecalculateTotal();
    }

    public int Count => _moneylist.Count;

    public decimal Total { get; protected set; }

    public event FinancialCollectionChangedEventHandler<TCollection, TEntry>? CollectionChanged;
    public event Action<TCollection, decimal>? TotalChanged;

    public virtual void Clear()
    {
        _moneylist.Clear();
        var diff = -Total;
        Total = 0;
        RaiseTotalChangedEvent(diff);
        RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Reset, default, false);
    }

    public bool Contains(TEntry item) => _moneylist.Contains(GetInnerEntry(item));

    public IEnumerator<TEntry> GetEnumerator() 
        => typeof(TInnerEntry).IsAssignableTo(typeof(TEntry))
            ? (IEnumerator<TEntry>)_moneylist.GetEnumerator()
            : _moneylist.Select(GetEntry).GetEnumerator();

    public void RecalculateTotal()
    {
        decimal total = 0;
        foreach (var x in _moneylist)
            total += GetEntry(x).Amount;
        Total = total;
    }

    public virtual bool Remove(TEntry item)
    {
        if (_moneylist.Remove(GetInnerEntry(item)))
        {
            Total -= item.Amount;
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Remove, item);
            return true;
        }

        return false;
    }

    protected virtual void RaiseTotalChangedEvent(decimal diff)
    {
        TotalChanged?.Invoke((TCollection)this, diff);
    }

    protected void RaiseCollectionChangedEvent(NotifyCollectionChangedAction action, TEntry? item, bool raiseTotalChanged = true)
    {
        if (raiseTotalChanged)
        {
            Debug.Assert(item is not null);
            RaiseTotalChangedEvent(item.Amount);
        }
        CollectionChanged?.Invoke((TCollection)this, action, item);
    }
}