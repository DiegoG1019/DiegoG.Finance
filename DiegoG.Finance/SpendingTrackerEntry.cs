using DiegoG.Finance.Internal;
using DiegoG.Finance.Results;
using MessagePack;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;

namespace DiegoG.Finance;

public sealed class SpendingTrackerEntry : FinancialEntity<SpendingTrackerSheet, SpendingTrackerEntry>, IFinanciallyAnnotated
{
    [MessagePackObject]
    public readonly record struct Info(
        [property: Key(1)] string ExpenseType, 
        [property: Key(2)] string ExpenseCategory, 
        [property: Key(0)] decimal Amount
    );

    internal SpendingTrackerTypeResult TrackerFactory(ExpenseType type)
    {
        Debug.Assert(Category.Parent == type);
        return new(Parent, type);
    }

    internal SpendingTrackerEntry(SpendingTrackerSheet parent, ExpenseCategory category, decimal amount = 0) : base(parent)
    {
        Debug.Assert(category != null);
        category.ThrowIfNotSameSheet(Sheet);
        Category = category;
        Amount = amount;
    }

    internal SpendingTrackerEntry(SpendingTrackerSheet parent, Info info) : base(parent)
    {
        Debug.Assert(string.IsNullOrWhiteSpace(info.ExpenseType) is false);
        Debug.Assert(string.IsNullOrWhiteSpace(info.ExpenseCategory) is false);
        
        if (parent.Sheet.ExpenseTypesAndCategories.TryGetValue(info.ExpenseType, out var type) is false)
            throw new ArgumentException($"Could not find an ExpenseType named '{info.ExpenseType}'", nameof(info));

        if (type.TryGetValue(info.ExpenseCategory, out var category) is false)
            throw new ArgumentException($"Could not find an ExpenseCategory named '{info.ExpenseCategory}' within ExpenseType '{info.ExpenseType}'", nameof(info));

        Category = category;
        Amount = info.Amount;
    }

    public ExpenseCategory Category { get; }

    protected override void OnInvalidate()
    {
        Category.DetachEntity(this);
        var tracker = Category.Parent.GetOrAddTracker(TrackerFactory);
        if (tracker is not null)
            tracker.Total -= Amount;
    }

    public decimal Amount
    {
        get;
        set
        {
            var diff = value - field;
            var tracker = Category.Parent.GetOrAddTracker(TrackerFactory);
            tracker.Total += diff;
            ChangeValue(ref field, value, Internal_AmountChanged, AmountChanged);
        }
    }

    public event FinancialWorkValueChangedHandler<SpendingTrackerEntry, decimal>? AmountChanged;
    internal event FinancialWorkValueChangedHandler<SpendingTrackerEntry, decimal>? Internal_AmountChanged;
}
