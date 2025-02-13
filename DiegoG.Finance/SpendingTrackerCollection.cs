using DiegoG.Finance.Internal;
using System.Diagnostics;

namespace DiegoG.Finance;

public class SpendingTrackerCollection : FinancialCollectionBase<SpendingTrackerCollection, SpendingTrackerEntry>
{
    public SpendingTrackerSheet Parent { get; }

    internal SpendingTrackerCollection(SpendingTrackerSheet parent, IEnumerable<SpendingTrackerEntry.Info>? values)
        : base(
            parent.Sheet,
            values is not null
                ? new InfoFactoryCollection<SpendingTrackerEntry.Info, SpendingTrackerEntry>(values, info => new SpendingTrackerEntry(parent, info))
                : null
        )
    {
        Debug.Assert(parent is not null);
        Parent = parent;
    }

    public virtual SpendingTrackerEntry Add(ExpenseCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);
        category.ThrowIfNotSameSheet(Sheet);
        var entry = new SpendingTrackerEntry(Parent, category);
        Set.Add(category, entry);
        FireCollectionEvent(System.Collections.Specialized.NotifyCollectionChangedAction.Add, entry);
        return entry;
    }

    public override void Clear()
    {
        foreach (var entry in Set)
        {
            entry.Value.Invalidate();
        }
        base.Clear();
    }

    public override bool Remove(SpendingTrackerEntry item)
    {
        if (base.Remove(item))
        {
            item.Invalidate();
            return true;
        }

        return false;
    }
}

public class SpendingTrackerIncomeCollection : SpendingTrackerCollection
{
    internal SpendingTrackerIncomeCollection(SpendingTrackerSheet parent, IEnumerable<SpendingTrackerEntry.Info>? values) 
        : base(parent, values) { }

    public override SpendingTrackerEntry Add(ExpenseCategory category)
        => category.Parent.Parent.Income == category.Parent
            ? throw new ArgumentException("The ExpenseCategory must be under the Income ExpenseType", nameof(category))
            : base.Add(category);
}
