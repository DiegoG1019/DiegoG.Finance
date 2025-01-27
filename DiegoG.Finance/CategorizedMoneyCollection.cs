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

public class CategorizedMoneyCollection : CategorizedFinancialCollection<MoneyCollection>
{
    private ReferredReference<MoneyCollectionTotalChangedEventHandler> Internal_Handler_MoneyTotalChanged;

    internal ReferredReference<FinancialCollectionChangedEventHandler<CategorizedMoneyCollection, KeyValuePair<string, MoneyCollection>>>? Internal_CollectionChanged;

    internal ReferredReference<CategorizedMoneyCollectionInnerCollectionChangedEventHandler>? Internal_InnerCollectionChanged;

    public delegate void CategorizedMoneyCollectionTotalChangedEventHandler(CategorizedMoneyCollection sender, MoneyCollection moneyCollection, decimal difference);

    public delegate void CategorizedMoneyCollectionInnerCollectionChangedEventHandler(CategorizedMoneyCollection sender, MoneyCollection moneyCollection, NotifyCollectionChangedAction action);

    internal CategorizedMoneyCollection(Dictionary<string, MoneyCollection> categories) : base(categories)
    {
        Internal_Handler_MoneyTotalChanged = new(Val_TotalChanged);

        RecalculateTotal();
        foreach (var x in _categories.Values)
            x.Internal_TotalChanged = Internal_Handler_MoneyTotalChanged;
    }

    public CategorizedMoneyCollection() : base()
    {
        Internal_Handler_MoneyTotalChanged = new(Val_TotalChanged);
    }

    public decimal Total { get; private set; }
    
    public void EnsureCapacity(int capacity)
        => _categories.EnsureCapacity(capacity);

    private void Val_TotalChanged(MoneyCollection collection, decimal difference)
    {
        Total += difference;
        TotalChanged?.Invoke(this, collection, difference);
    }

    protected override MoneyCollection ValueFactory(string key)
        => new()
        {
            Internal_TotalChanged = Internal_Handler_MoneyTotalChanged,
            Category = key
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
        Internal_Handler_MoneyTotalChanged = new(Val_TotalChanged);
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
            Val_TotalChanged(value, -value.Total);
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
        Internal_CollectionChanged?.Value?.Invoke(this, action, item);
        CollectionChanged?.Invoke(this, action, item);
    }

    public event CategorizedMoneyCollectionTotalChangedEventHandler? TotalChanged;
    public event FinancialCollectionChangedEventHandler<CategorizedMoneyCollection, KeyValuePair<string, MoneyCollection>>? CollectionChanged;
}
