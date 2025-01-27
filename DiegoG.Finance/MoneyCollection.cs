using NodaMoney;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace DiegoG.Finance;

public class MoneyCollection : IReadOnlyCollection<LabeledAmount>
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
    internal ReferredReference<MoneyCollectionTotalChangedEventHandler>? Internal_TotalChanged;
    internal ReferredReference<FinancialWorkEventHandler<LabeledAmount, decimal>> Internal_Handler_AmountChanged;

    internal MoneyCollection(IEnumerable<LabeledAmount>? amounts = null)
    {
        Internal_Handler_AmountChanged = new(LabeledAmountChanged);
        _moneylist = amounts is null
                        ? new()
                        : new(amounts);

        RecalculateTotal();
    }

    public MoneyCollection()
    {
        Internal_Handler_AmountChanged = new(LabeledAmountChanged);
        _moneylist = [];

        RecalculateTotal();
    }

#warning When an owner CategorizedMoneyCollection is cleared, Category is not updated. It would turn "Clear" into a O(n) operation, which is not ideal
    public string? Category { get; internal set; }

    public decimal Total { get; private set; }

    public LabeledAmount Add(string label, decimal amount)
    {
        var item = new LabeledAmount(label, amount)
        {
            Internal_AmountChanged = Internal_Handler_AmountChanged
        };

        _moneylist.Add(item);
        Total += item.Amount;
        RaiseEvent(NotifyCollectionChangedAction.Add, item);
        return item;
    }

    public void Clear()
    {
        _moneylist.Clear();
        var diff = -Total;
        Total = 0;
        Internal_TotalChanged?.Value?.Invoke(this, diff);
        TotalChanged?.Invoke(this, diff);
        RaiseEvent(NotifyCollectionChangedAction.Reset, default, false);
    }

    public bool Remove(LabeledAmount item)
    {
        if (_moneylist.Remove(item))
        {
            Total -= item.Amount;
            RaiseEvent(NotifyCollectionChangedAction.Remove, item);
            return true;
        }

        return false;
    }

    public void RecalculateTotal()
    {
        decimal total = 0;
        foreach (var x in _moneylist)
            total += x.Amount;
        Total = total;
    }

    private void RaiseEvent(NotifyCollectionChangedAction action, LabeledAmount? item, bool raiseTotalChanged = true)
    {
        if (raiseTotalChanged)
        {
            Debug.Assert(item is not null);
            Internal_TotalChanged?.Value?.Invoke(this, item.Amount);
            TotalChanged?.Invoke(this, item.Amount);
        }
        CollectionChanged?.Invoke(this, action, item);
    }

    public bool Contains(LabeledAmount item) => _moneylist.Contains(item);

    public void CopyTo(LabeledAmount[] array, int arrayIndex) => _moneylist.CopyTo(array, arrayIndex);

    public int Count => _moneylist.Count;

    public IEnumerator<LabeledAmount> GetEnumerator() => _moneylist.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _moneylist.GetEnumerator();

    public event FinancialCollectionChangedEventHandler<MoneyCollection, LabeledAmount>? CollectionChanged;

    private void LabeledAmountChanged(LabeledAmount amount, decimal old, decimal @new)
    {
        var diff = @new - old;
        Total += diff;
        Internal_TotalChanged?.Value?.Invoke(this, diff);
        TotalChanged?.Invoke(this, diff);
        CollectionChanged?.Invoke(this, NotifyCollectionChangedAction.Replace, amount);
    }
}
