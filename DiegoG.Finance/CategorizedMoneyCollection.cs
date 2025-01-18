using MessagePack;
using NodaMoney;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using static DiegoG.Finance.MoneyCollection;

namespace DiegoG.Finance;

public class CategorizedMoneyCollection : CategorizedFinancialCollection<MoneyCollection>, IFinancialWork
{
    private ReferredEventReference<MoneyCollectionTotalChangedEventHandler> MoneyTotalChanged;
    internal IFinancialWork? Parent;

    public delegate void CategorizedMoneyCollectionTotalChangedEventHandler(CategorizedMoneyCollection sender, MoneyCollection moneyCollection, decimal difference);

    internal CategorizedMoneyCollection(IFinancialWork parent, Dictionary<string, MoneyCollection> categories) : base(categories)
    {
        MoneyTotalChanged = new(Val_TotalChanged);
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        if (Parent == this)
            throw new ArgumentException("The CategorizedMoneyCollection's parent cannot be the same instance", nameof(parent));
        RecalculateTotal();
        foreach (var x in _categories.Values)
        {
            x.Internal_totalChanged = MoneyTotalChanged;
            x.Parent = this;
        }
    }

    internal CategorizedMoneyCollection(Currency currency, Dictionary<string, MoneyCollection> categories) : base(categories)
    {
        MoneyTotalChanged = new(Val_TotalChanged);
        Currency = currency;
        RecalculateTotal();
        foreach (var x in _categories.Values)
        {
            x.Internal_totalChanged = MoneyTotalChanged;
            x.Parent = this;
        }
    }

    public CategorizedMoneyCollection(IFinancialWork parent) : base()
    {
        MoneyTotalChanged = new(Val_TotalChanged);
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
    }

    public CategorizedMoneyCollection(Currency currency) : base()
    {
        MoneyTotalChanged = new(Val_TotalChanged);
        Currency = currency;
    }

    public Currency Currency => Parent?.Currency ?? field;

    public decimal Total { get; private set; }

    public Money MoneyTotal => new(Total, Currency);

    public event CategorizedMoneyCollectionTotalChangedEventHandler? TotalChanged;

    public void EnsureCapacity(int capacity)
        => _categories.EnsureCapacity(capacity);

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
            total += x.Total;
        Total = total;
    }
}
