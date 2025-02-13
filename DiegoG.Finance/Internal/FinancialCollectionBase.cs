using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace DiegoG.Finance.Internal;

public abstract class FinancialCollectionBase<TCollection, TValue> : FinancialWork<TCollection>, IReadOnlyCollection<TValue>
    where TCollection : FinancialCollectionBase<TCollection, TValue>
    where TValue : FinancialWork<TValue>, IFinanciallyAnnotated
{
    protected readonly Dictionary<ExpenseCategory, TValue> Set;

    internal FinancialCollectionBase(WorkSheet sheet, IEnumerable<TValue>? values) : base(sheet)
    {
        Set = values?.ToDictionary(k => k.Category, v => v) ?? [];
    }

    public decimal Total { get; protected set; }

    public void RecalculateTotal()
    {
        var old = Total;
        decimal total = 0;
        foreach (var x in Set)
            total += x.Value.Amount;

        if (old != total)
        {
            Total = total;
            FireTotalEvent(old, total);
        }
    }

    public event FinancialWorkValueChangedHandler<TCollection, decimal>? TotalChanged;
    internal event FinancialWorkValueChangedHandler<TCollection, decimal>? Internal_TotalChanged;

    public event FinancialCollectionChangedEventHandler<TCollection, TValue>? CollectionChanged;
    internal event FinancialCollectionChangedEventHandler<TCollection, TValue>? Internal_CollectionChanged;

    public int Count => Set.Count;

    public IEnumerator<TValue> GetEnumerator()
        => Set.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => Set.Values.GetEnumerator();

    public virtual void Clear()
    {
        Set.Clear();
        FireCollectionEvent(NotifyCollectionChangedAction.Reset, null);
    }

    public virtual bool Remove(TValue item)
    {
        if (Set.Remove(item.Category))
        {
            FireCollectionEvent(NotifyCollectionChangedAction.Remove, item);
            return true;
        }
        return false;
    }

    protected void FireTotalEvent(decimal old, decimal @new)
    {
        TotalChanged?.Invoke((TCollection)this, old, @new);
        Internal_TotalChanged?.Invoke((TCollection)this, old, @new);
    }

    protected void FireCollectionEvent(NotifyCollectionChangedAction action, TValue? item)
    {
        Internal_CollectionChanged?.Invoke((TCollection)this, action, item);
        CollectionChanged?.Invoke((TCollection)this, action, item);
    }

    public bool Contains(TValue item)
        => item.Sheet == Sheet && Set.ContainsKey(item.Category);
}
