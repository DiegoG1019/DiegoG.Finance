using NodaMoney;
using System.Collections;
using System.ComponentModel.Design;
using System.Text.Json.Serialization;

namespace DiegoG.Finance;

public class MoneyCollection : ISet<LabeledAmount>, IReadOnlySet<LabeledAmount>, IEnumerable<LabeledMoney>, IFinancialWork
{
    internal protected readonly HashSet<LabeledAmount> _moneylist;

    public delegate void MoneyCollectionTotalChangedEventHandler(MoneyCollection sender, decimal difference);
    public delegate void MoneyCollectionCurrencyChangedEventHandler(MoneyCollection sender, Currency newCurrency);

    /// <summary>
    /// This event is fired whenever <see cref="Total"/> changes
    /// </summary>
    /// <remarks>
    /// Note that clearing this event (such as via <c><see cref="TotalChanged"/> = <see langword="null"/></c>) will <i>not</i> affect <see cref="CategorizedMoneyCollection"/>, as they will still see the changes in <see cref="Total"/>
    /// </remarks>
    public event Action<MoneyCollection, decimal>? TotalChanged;
    internal ReferredEventReference<MoneyCollectionTotalChangedEventHandler>? Internal_totalChanged;
    internal IFinancialWork? Parent;

    public MoneyCollection(IFinancialWork parent, IEnumerable<LabeledAmount>? amounts = null)
    {
        _moneylist = amounts is null
                        ? new(LabeledAmount.LabeledAmountComparer.Instance)
                        : new(amounts, LabeledAmount.LabeledAmountComparer.Instance);

        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        if (Parent == this)
            throw new ArgumentException("The MoneyCollection's parent cannot be the same instance", nameof(parent));
        RecalculateTotal();
    }

    public MoneyCollection(Currency currency, IEnumerable<LabeledAmount>? amounts = null)
    {
        _moneylist = amounts is null
                        ? new(LabeledAmount.LabeledAmountComparer.Instance)
                        : new(amounts, LabeledAmount.LabeledAmountComparer.Instance);

        Currency = currency;
        RecalculateTotal();
    }

    public IEnumerable<LabeledMoney> GetAmountsAsMoney()
    {
        foreach (var mon in _moneylist)
            yield return new LabeledMoney(mon.Label, new(mon.Amount, Currency));
    }

    public decimal Total { get; private set; }

    public Currency Currency => Parent?.Currency ?? field;

    public Money MoneyTotal => new(Total, Currency);

    public bool Add(LabeledAmount item)
    {
        if (_moneylist.Add(item))
        {
            Total += item.Amount;
            RaiseEvent(item.Amount);
            return true;
        }
        return false;
    }

    public void Clear()
    {
        _moneylist.Clear();
        Total = 0;
        TotalChanged?.Invoke(this, 0);
    }

    public bool Remove(LabeledAmount item)
    {
        if (_moneylist.Remove(item))
        {
            Total -= item.Amount;
            RaiseEvent(item.Amount);
            return true;
        }

        return false;
    }

    public void ExceptWith(IEnumerable<LabeledAmount> other)
    {
        ArgumentNullException.ThrowIfNull(other);

        // This is already the empty set; return.
        if (Count == 0)
            return;

        // Special case if other is this; a set minus itself is the empty set.
        if (other == this)
        {
            Clear();
            return;
        }

        // Remove every element in other from this.
        foreach (LabeledAmount element in other)
        {
            Remove(element);
        }
    }

    public void IntersectWith(IEnumerable<LabeledAmount> other)
    {
        ArgumentNullException.ThrowIfNull(other);

        // Intersection of anything with empty set is empty set, so return if count is 0.
        // Same if the set intersecting with itself is the same set.
        if (Count == 0 || other == this)
            return;

        // If other is known to be empty, intersection is empty set; remove all elements, and we're done.
        if (other.Any() is false)
        {
            Clear();
            return;
        }

        _moneylist.IntersectWith(other);
        RecalculateTotal();
    }

    public void SymmetricExceptWith(IEnumerable<LabeledAmount> other)
    {
        ArgumentNullException.ThrowIfNull(other);

        // If set is empty, then symmetric difference is other.
        if (Count == 0)
        {
            UnionWith(other);
            return;
        }

        // Special-case this; the symmetric difference of a set with itself is the empty set.
        if (other == this)
        {
            Clear();
            return;
        }

        _moneylist.SymmetricExceptWith(other);
        RecalculateTotal();
    }

    public void UnionWith(IEnumerable<LabeledAmount> other)
    {
        ArgumentNullException.ThrowIfNull(other);

        foreach (var item in other)
        {
            if (Add(item))
                Total += item.Amount;
        }
    }

    public void RecalculateTotal()
    {
        decimal total = 0;
        foreach (var x in _moneylist)
            total += x.Amount;
        Total = total;
    }

    private void RaiseEvent(decimal diff)
    {
        Internal_totalChanged?.Event?.Invoke(this, diff);
        TotalChanged?.Invoke(this, diff);
    }

    void ICollection<LabeledAmount>.Add(LabeledAmount item)
        => Add(item);

    public bool IsProperSubsetOf(IEnumerable<LabeledAmount> other) => _moneylist.IsProperSubsetOf(other);

    public bool IsProperSupersetOf(IEnumerable<LabeledAmount> other) => _moneylist.IsProperSupersetOf(other);

    public bool IsSubsetOf(IEnumerable<LabeledAmount> other) => _moneylist.IsSubsetOf(other);

    public bool IsSupersetOf(IEnumerable<LabeledAmount> other) => _moneylist.IsSupersetOf(other);

    public bool Overlaps(IEnumerable<LabeledAmount> other) => _moneylist.Overlaps(other);

    public bool SetEquals(IEnumerable<LabeledAmount> other) => _moneylist.SetEquals(other);

    public bool Contains(LabeledAmount item) => _moneylist.Contains(item);

    public void CopyTo(LabeledAmount[] array, int arrayIndex) => _moneylist.CopyTo(array, arrayIndex);

    public int Count => _moneylist.Count;

    bool ICollection<LabeledAmount>.IsReadOnly => false;

    public IEnumerator<LabeledAmount> GetEnumerator() => _moneylist.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _moneylist.GetEnumerator();

    IEnumerator<LabeledMoney> IEnumerable<LabeledMoney>.GetEnumerator()
    {
        foreach (var item in _moneylist)
            yield return new LabeledMoney(item.Label, new(item.Amount, Currency));
    }
}
