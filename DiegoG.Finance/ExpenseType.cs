using DiegoG.Finance.Internal;
using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace DiegoG.Finance;

public sealed class ExpenseType : CategoryBase<ExpenseTypesCollection, ExpenseType>, IReadOnlyCollection<ExpenseCategory>
{
    internal readonly Dictionary<string, ExpenseCategory> _categories;

    [MessagePackObject]
    public readonly record struct Info(
        [property: Key(1)] string Name,
        [property: Key(0)] bool IsIncome,
        [property: Key(2)] IEnumerable<ExpenseCategory.Info>? Categories
    );

    internal ExpenseType(ExpenseTypesCollection parent, string name, IEnumerable<ExpenseCategory.Info>? categories)
        : base(parent)
    {
        Debug.Assert(string.IsNullOrWhiteSpace(name) is false);

        var dict = new Dictionary<string, ExpenseCategory>();
        if (categories is not null)
        {
            foreach (var info in categories)
            {
                if (string.IsNullOrWhiteSpace(info.Name))
                    throw new ArgumentException("One or more ExpenseCategories have a name that is null or only whitespace", nameof(categories));

                var et = new ExpenseCategory(this, info.Name);
                if (dict.TryAdd(info.Name, et) is false)
                    throw new ArgumentException($"More than one ExpenseCategory named '{info.Name}' exists within ExpenseType '{name}'", nameof(categories));
            }
        }

        _categories = dict;
        Name = name;
    }

    public string Name { get; private set; }

    public bool Add(string name, [NotNullWhen(true)] out ExpenseCategory? category)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_categories.ContainsKey(name))
        {
            category = null;
            return false;
        }

        category = new(this, name);
        _categories.Add(name, category);
        return true;
    }

    public bool TryGetValue(string name, [NotNullWhen(true)] out ExpenseCategory? category)
        => _categories.TryGetValue(name, out category);

    public bool Remove(string name) 
        => _categories.Remove(name);

    public bool TryRename(string newName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);
        if (Parent is not ExpenseTypesCollection parent)
            return false;

        var old = Name;

        if (parent._types.TryAdd(newName, this))
        {
            Name = newName;
            parent._types.Remove(old);
            NameChanged?.Invoke(this, old, newName);
            return true;
        }

        return false;
    }

    public int Count => _categories.Count;

    public IEnumerator<ExpenseCategory> GetEnumerator()
        => _categories.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _categories.Values.GetEnumerator();

    public event FinancialWorkValueChangedHandler<ExpenseType, string>? NameChanged;
}
