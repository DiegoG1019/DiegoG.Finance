using MessagePack;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using NodaMoney;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using static DiegoG.Finance.MoneyCollection;

namespace DiegoG.Finance;

public class CategorizedMoneyCollection : CategorizedFinancialCollection<MoneyCollection>, IFinancialWork, IInternalMoneyCollectionParent
{
    private ReferredReference<MoneyCollectionTotalChangedEventHandler> MoneyTotalChanged;

    internal ReferredReference<FinancialCollectionChangedEventHandler<CategorizedMoneyCollection, KeyValuePair<string, MoneyCollection>>>? Internal_CollectionChanged;

    internal ReferredReference<CategorizedMoneyCollectionInnerCollectionChangedEventHandler>? Internal_InnerCollectionChanged;
    internal ReferredReference<Action<MoneyCollection, LabeledAmount>> __internal_MoneyCollectionEventPropagationHandler;

    internal IInternalCategorizedMoneyCollectionParent? Parent;

    public delegate void CategorizedMoneyCollectionTotalChangedEventHandler(CategorizedMoneyCollection sender, MoneyCollection moneyCollection, decimal difference);

    public delegate void CategorizedMoneyCollectionInnerCollectionChangedEventHandler(CategorizedMoneyCollection sender, MoneyCollection moneyCollection, NotifyCollectionChangedAction action);

    internal CategorizedMoneyCollection(Currency currency, Dictionary<string, MoneyCollection> categories) : base(categories)
    {
        MoneyTotalChanged = new(Val_TotalChanged);
        __internal_MoneyCollectionEventPropagationHandler = new(moneyCollectionEventPropagationHandler);

        Currency = currency;
        RecalculateTotal();
        foreach (var x in _categories.Values)
        {
            x.Internal_TotalChanged = MoneyTotalChanged;
            x.Parent = this;
        }
    }

    public CategorizedMoneyCollection(Currency currency) : base()
    {
        MoneyTotalChanged = new(Val_TotalChanged);
        __internal_MoneyCollectionEventPropagationHandler = new(moneyCollectionEventPropagationHandler);

        Currency = currency;
    }

    public Currency Currency => Parent?.Currency ?? field;

    public decimal Total { get; private set; }

    public Money MoneyTotal => new(Total, Currency);

    public void EnsureCapacity(int capacity)
        => _categories.EnsureCapacity(capacity);

    private void Val_TotalChanged(MoneyCollection collection, decimal difference)
    {
        Total += difference;
        TotalChanged?.Invoke(this, collection, difference);
    }

    protected override MoneyCollection ValueFactory(string key)
        => new(Currency)
        {
            Internal_TotalChanged = MoneyTotalChanged,
            Category = key,
            Parent = this
        };

    public override MoneyCollection Add(string key)
    {
        var coll = base.Add(key);
        RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Add, new(key, coll));
        return coll;
    }

    public override void Clear()
    {
        base.Clear();
        MoneyTotalChanged = new(Val_TotalChanged);
        __internal_MoneyCollectionEventPropagationHandler = new(moneyCollectionEventPropagationHandler);
        RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Reset, default);
    }

    public bool Rename(string key, string newName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);

        if (_categories.Comparer.Equals(key, newName))
            return true;

        if (_categories.Remove(key, out var collection))
        {
            _categories.Add(key, collection);
            collection.Category = newName;
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Move, new(key, collection));
            return true;
        }

        return false;
    }

    public override bool Remove(string key, [MaybeNullWhen(false)] out MoneyCollection value)
    {
        if (base.Remove(key, out value)) 
        {
            value.Internal_TotalChanged = null;
            value.Category = null;
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Remove, new(key, value));
            return true;
        }

        value = null;
        return false;
    }

    public void RecalculateTotal()
    {
        decimal total = 0;
        foreach (var x in _categories)
        {
            total += x.Value.Total;
            x.Value.Category = x.Key;
        }
        Total = total;
    }

    private void RaiseCollectionChangedEvent(NotifyCollectionChangedAction action, KeyValuePair<string, MoneyCollection> item)
    {
        Parent?.Internal_MemberChanged?.Value?.Invoke(this, item.Value);
        Internal_CollectionChanged?.Value?.Invoke(this, action, item);
        CollectionChanged?.Invoke(this, action, item);
    }

    private void RaiseCategorizedMoneyCollectionMemberEvent(CategorizedMoneyCollection collection, KeyValuePair<string, MoneyCollection> item)
    {
        Parent?.Internal_MemberChanged?.Value?.Invoke(this, item.Value);
        CollectionChanged?.Invoke(this, NotifyCollectionChangedAction.Replace, item);
    }

    private void moneyCollectionEventPropagationHandler(MoneyCollection c, LabeledAmount l)
        => RaiseCategorizedMoneyCollectionMemberEvent(this, new (c.Category!, c));

    public event CategorizedMoneyCollectionTotalChangedEventHandler? TotalChanged;
    public event FinancialCollectionChangedEventHandler<CategorizedMoneyCollection, KeyValuePair<string, MoneyCollection>>? CollectionChanged;

    ReferredReference<Action<MoneyCollection, LabeledAmount>> IInternalMoneyCollectionParent.Internal_MemberChanged
        => __internal_MoneyCollectionEventPropagationHandler;
}
