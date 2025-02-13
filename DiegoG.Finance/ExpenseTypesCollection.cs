using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Collections;
using DiegoG.Finance.Internal;

namespace DiegoG.Finance;

public sealed class ExpenseTypesCollection : FinancialWork<ExpenseTypesCollection>, IReadOnlyCollection<ExpenseType>
{
    internal readonly Dictionary<string, ExpenseType> _types;

    internal ExpenseType.Info InfoSelector(ExpenseType type)
        => new(type.Name, type == Income, type.Select(x => new ExpenseCategory.Info(x.Name)));

    internal IEnumerable<ExpenseType.Info> GetInfo()
        => this.Select(InfoSelector);

    internal ExpenseTypesCollection(WorkSheet sheet, IEnumerable<ExpenseType.Info>? types) : base(sheet)
    {
        var dict = new Dictionary<string, ExpenseType>();
        if (types is not null)
        {
            foreach (var info in types)
            {
                if (string.IsNullOrWhiteSpace(info.Name))
                    throw new ArgumentException("One or more ExpenseTypes have a name that is null or only whitespace", nameof(types));

                var et = new ExpenseType(this, info.Name, info.Categories);
                if (dict.TryAdd(info.Name, et) is false)
                    throw new ArgumentException($"More than one ExpenseType named '{info.Name}' exists", nameof(types));
                if (info.IsIncome)
                {
                    if (Income is not null)
                        throw new ArgumentException($"More than one ExpenseType is marked as income type", nameof(types));
                    Income = et;
                }
            }
        }
        
        if (Income is null)
        {
            if (dict.TryGetValue("Income", out var income))
                Income = income;
            else
            {
                Income = new(this, "Income", null!);
                dict.Add(Income.Name, Income);
            }
        }

        _types = dict;
    }

    public ExpenseType Income { get; }

    public bool Add(string name, [NotNullWhen(true)] out ExpenseType? type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_types.ContainsKey(name))
        {
            type = null;
            return false;
        }

        type = new(this, name, null);
        _types.Add(name, type);
        return true;
    }

    public bool TryGetValue(string name, [NotNullWhen(true)] out ExpenseType? type)
        => _types.TryGetValue(name, out type);

    public bool Remove(string name)
        => _types.Remove(name);

    public int Count => _types.Count;

    public IEnumerator<ExpenseType> GetEnumerator()
        => _types.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _types.Values.GetEnumerator();
}
