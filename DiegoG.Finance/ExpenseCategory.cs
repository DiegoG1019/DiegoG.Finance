using DiegoG.Finance.Internal;
using MessagePack;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace DiegoG.Finance;

public sealed class ExpenseCategory : CategoryBase<ExpenseType, ExpenseCategory>
{
    [MessagePackObject]
    public readonly record struct Info([property: Key(0)] string Name);

    public string Name { get; private set; }

    internal ExpenseCategory(ExpenseType parent, string name) : base(parent)
    {
        Debug.Assert(string.IsNullOrWhiteSpace(name) is false);
        Name = name;
    }

    public bool TryRename(string newName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);
        if (Parent is not ExpenseType parent)
            return false;

        var old = Name;

        if (parent._categories.TryAdd(newName, this))
        {
            Name = newName;
            parent._categories.Remove(old);
            NameChanged?.Invoke(this, old, newName);
            return true;
        }

        return false;
    }

    public event FinancialWorkValueChangedHandler<ExpenseCategory, string>? NameChanged;
}
