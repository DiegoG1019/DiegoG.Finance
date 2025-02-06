using MessagePack;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using NodaMoney;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DiegoG.Finance;

public class ExpenseTypesCollection : CategorizedFinancialCollection<CategorizedMoneyCollection>
{
    private ReferredReference<MoneyCollectionTotalChangedEventHandler<CategorizedMoneyCollection, ExpenseCategory, Dictionary<string, ExpenseCategory>, KeyValuePair<string, ExpenseCategory>>> Internal_Handler_MoneyTotalChanged;

    internal ReferredReference<FinancialCollectionChangedEventHandler<ExpenseTypesCollection, KeyValuePair<string, CategorizedMoneyCollection>>>? Internal_CollectionChanged;

    internal ReferredReference<CategorizedMoneyCollectionInnerCollectionChangedEventHandler>? Internal_InnerCollectionChanged;

    public delegate void CategorizedMoneyCollectionTotalChangedEventHandler(ExpenseTypesCollection sender, CategorizedMoneyCollection moneyCollection, decimal difference);

    public delegate void CategorizedMoneyCollectionInnerCollectionChangedEventHandler(ExpenseTypesCollection sender, CategorizedMoneyCollection moneyCollection, NotifyCollectionChangedAction action);

    internal ExpenseTypesCollection(Dictionary<string, CategorizedMoneyCollection> categories) : base(categories)
    {
        Internal_Handler_MoneyTotalChanged = new(Val_TotalChanged);

        RecalculateTotal();
        foreach (var x in _categories.Values)
            x.Internal_TotalChanged = Internal_Handler_MoneyTotalChanged;
    }

    public ExpenseTypesCollection() : base()
    {
        Internal_Handler_MoneyTotalChanged = new(Val_TotalChanged);
    }

    internal FinancialCategoryCollection? CategoryInfo { get; init; }

    public decimal Total { get; private set; }
    
    public void EnsureCapacity(int capacity)
        => _categories.EnsureCapacity(capacity);

    private void Val_TotalChanged(CategorizedMoneyCollection collection, decimal difference)
    {
        Total += difference;
        TotalChanged?.Invoke(this, collection, difference);
    }

    protected override CategorizedMoneyCollection ValueFactory(string key)
        => new(key)
        {
            Internal_TotalChanged = Internal_Handler_MoneyTotalChanged,
            CategoryInfo = CategoryInfo
        };

    public override CategorizedMoneyCollection Add(string key)
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

        if (_categories.TryGetValue(key, out var collection) && collection.TryRenameExpenseType(newName))
        {
            _categories.Add(newName, collection);
            _categories.Remove(key);
            RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Move, new(key, collection));
            return true;
        }

        return false;
    }

    public override bool Remove(string key, [MaybeNullWhen(false)] out CategorizedMoneyCollection value)
    {
        if (base.Remove(key, out value)) 
        {
            value.ClearFromRecord();
            Val_TotalChanged(value, -value.Total);
            value.Internal_TotalChanged = null;
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
            total += x.Value.Total;
        Total = total;
    }

    private void RaiseCollectionChangedEvent(NotifyCollectionChangedAction action, KeyValuePair<string, CategorizedMoneyCollection> item)
    {
        Internal_CollectionChanged?.Value?.Invoke(this, action, item);
        CollectionChanged?.Invoke(this, action, item);
    }

    public event CategorizedMoneyCollectionTotalChangedEventHandler? TotalChanged;
    public event FinancialCollectionChangedEventHandler<ExpenseTypesCollection, KeyValuePair<string, CategorizedMoneyCollection>>? CollectionChanged;
}
