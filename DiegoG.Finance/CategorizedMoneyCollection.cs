using NodaMoney;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using static DiegoG.Finance.MoneyCollection;

namespace DiegoG.Finance;

public class CategorizedMoneyCollection : CategorizedFinancialCollection<MoneyCollection>
{
    private ReferredEventReference<MoneyCollectionTotalChangedEventHandler> MoneyTotalChanged;

    public delegate void CategorizedMoneyCollectionTotalChangedEventHandler(CategorizedMoneyCollection sender, MoneyCollection moneyCollection, decimal difference);

    internal CategorizedMoneyCollection(Currency currency, Dictionary<string, MoneyCollection> categories) : base(categories)
    {
        MoneyTotalChanged = new(Val_TotalChanged);
        Currency = currency;
        RecalculateTotal();
    }

    public CategorizedMoneyCollection(Currency currency) : base()
    {
        MoneyTotalChanged = new(Val_TotalChanged);
        Currency = currency;
    }

    public CategorizedMoneyCollection(Currency currency, int capacity) : base(capacity)
    {
        MoneyTotalChanged = new(Val_TotalChanged);
        Currency = currency;
    }

    public Currency Currency { get; }

    public decimal Total { get; private set; }

    public Money MoneyTotal => new(Total, Currency);

    public event CategorizedMoneyCollectionTotalChangedEventHandler? TotalChanged;

    private void Val_TotalChanged(MoneyCollection collection, decimal difference)
    {
        Total += difference;
        TotalChanged?.Invoke(this, collection, difference);
    }

    protected override MoneyCollection ValueFactory(string key)
        => new(Currency) { Internal_totalChanged =  MoneyTotalChanged };

    public override void Clear()
    {
        base.Clear();
        MoneyTotalChanged = new(Val_TotalChanged);
    }

    public override bool Remove(string key, [MaybeNullWhen(false)] out MoneyCollection value)
    {
        if (base.Remove(key, out value)) 
        {
            value.Internal_totalChanged = null;
            return true;
        }

        value = null;
        return false;
    }

    public void RecalculateTotal()
    {
        decimal total = 0;
        foreach (var x in _categories.Values)
        {
            total += x.Total;
            x.Internal_totalChanged = MoneyTotalChanged;
        }
        Total = total;
    }
}
