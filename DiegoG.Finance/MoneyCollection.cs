using NodaMoney;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Formats.Tar;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace DiegoG.Finance;

public class MoneyCollection : MoneyCollectionBase<MoneyCollection, LabeledAmount>, IReadOnlyCollection<LabeledAmount>
{
    internal ReferredReference<FinancialWorkEventHandler<LabeledAmount, decimal>> Internal_Handler_AmountChanged;
    internal ReferredReference<MoneyCollectionTotalChangedEventHandler<MoneyCollection, LabeledAmount>>? Internal_TotalChanged;

    internal MoneyCollection(IEnumerable<LabeledAmount>? amounts = null) : base(amounts)
    {
        Internal_Handler_AmountChanged = new(LabeledAmountChanged);
    }

    public MoneyCollection()
    {
        Internal_Handler_AmountChanged = new(LabeledAmountChanged);
    }

    public LabeledAmount Add(string label, decimal amount)
    {
        var item = new LabeledAmount(label, amount)
        {
            Internal_AmountChanged = Internal_Handler_AmountChanged
        };

        _moneylist.Add(item);
        Total += item.Amount;
        RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Add, item);
        return item;
    }

#warning When an owner CategorizedMoneyCollection is cleared, Category is not updated. It would turn "Clear" into a O(n) operation, which is not ideal
    public string? Category { get; internal set; }

    IEnumerator IEnumerable.GetEnumerator() => _moneylist.GetEnumerator();

    private void LabeledAmountChanged(LabeledAmount amount, decimal old, decimal @new)
    {
        var diff = @new - old;
        Total += diff;
        RaiseTotalChangedEvent(diff);
        RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Replace, amount, false);
    }

    protected override void RaiseTotalChangedEvent(decimal diff)
    {
        Internal_TotalChanged?.Value?.Invoke(this, diff);
        base.RaiseTotalChangedEvent(diff);
    }

    protected override HashSet<LabeledAmount> InnerCollectionFactory(IEnumerable<LabeledAmount>? amounts)
        => amounts is null ? [] : new HashSet<LabeledAmount>(amounts);
}
