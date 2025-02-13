using DiegoG.Finance.Internal;
using MessagePack;
using System.Diagnostics;

namespace DiegoG.Finance;

public sealed class MoneyMovementEntry : PagedEntity<MoneyMovementTracker, MoneyMovementEntry>
{
    internal sealed class Comparer : IComparer<MoneyMovementEntry>
    {
        private Comparer() { }

        public static readonly IComparer<MoneyMovementEntry> Instance = new Comparer();

        public int Compare(MoneyMovementEntry? x, MoneyMovementEntry? y) 
            => x == y ? 0 : x is null ? -1 : y is null ? 1 : x.Date.CompareTo(y.Date);
    }

    [MessagePackObject]
    public readonly record struct Info(
        [property: Key(0)] DateTime Date, 
        [property: Key(1)] decimal Amount, 
        [property: Key(2)] string ExpenseType, 
        [property: Key(3)] string ExpenseCategory
    );

    public Info GetInfo()
        => new(
            Date,
            Amount,
            Category.Parent.Name,
            Category.Name
        );

    internal MoneyMovementEntry(MoneyMovementTracker parent, ExpenseCategory category, DateTime date) : base(parent)
    {
        Debug.Assert(category is not null);
        Date = date;
        Category = category;
    }

    internal MoneyMovementEntry(MoneyMovementTracker parent, Info info) : base(parent)
    {
        Date = info.Date;
        Amount = info.Amount;
        if (Sheet.ExpenseTypesAndCategories.TryGetValue(info.ExpenseType, out var et) is false)
            throw new ArgumentException($"Could not find an ExpenseType named '{info.ExpenseType}'", nameof(info));

        if (et.TryGetValue(info.ExpenseCategory, out var cat) is false)
            throw new ArgumentException(
                $"Could not find an ExpenseCategory named '{info.ExpenseCategory}' under ExpenseType '{info.ExpenseType}'", nameof(info)
            );
        
        Category = cat;
    }

    public DateTime Date
    {
        get;
        set
        {
            if (field == value) return;
            ChangeValue(ref field, value, Internal_DateChanged, DateChanged);
        }
    }

    public decimal Amount    
    {
        get;
        set
        {
            if (field == value) return;
            ChangeValue(ref field, value, Internal_AmountChanged, AmountChanged);
        }
    }

    public ExpenseCategory Category
    {
        get;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            ThrowIfNotSameSheet(value.Sheet);
            if (field == value) return;

            if (value.Parent == field.Parent)
            {
                if (Page.TryGetCategoryTracker<MoneyMovementTotalTracker>(field, out var oldCategoryTracker)) oldCategoryTracker.Detach(this);
                Page.GetOrAddCategoryTracker<MoneyMovementTotalTracker>(value, MoneyMovementTotalTracker.TrackerFactory).Attach(this);
            }
            else
            {
                if (Page.TryGetCategoryTracker<MoneyMovementTotalTracker>(field, out var oldCategoryTracker)) oldCategoryTracker.Detach(this);
                Page.GetOrAddCategoryTracker<MoneyMovementTotalTracker>(value, MoneyMovementTotalTracker.TrackerFactory).Attach(this);

                if (Page.TryGetTypeTracker<MoneyMovementTotalTracker>(field.Parent, out var oldTypeTracker)) oldTypeTracker.Detach(this);
                Page.GetOrAddTypeTracker<MoneyMovementTotalTracker>(value.Parent, MoneyMovementTotalTracker.TrackerFactory).Attach(this);
            }

            ChangeValue(ref field, value, null, CategoryChanged);
        }
    }

    protected override void OnInvalidate()
    {
        if (Page.TryGetCategoryTracker<MoneyMovementTotalTracker>(Category, out var oldCategoryTracker)) oldCategoryTracker.Detach(this);
        if (Page.TryGetTypeTracker<MoneyMovementTotalTracker>(Category.Parent, out var oldTypeTracker)) oldTypeTracker.Detach(this);
    }

    internal event FinancialWorkValueChangedHandler<MoneyMovementEntry, DateTime>? Internal_DateChanged;
    internal event FinancialWorkValueChangedHandler<MoneyMovementEntry, decimal>? Internal_AmountChanged;

    public event FinancialWorkValueChangedHandler<MoneyMovementEntry, ExpenseCategory>? CategoryChanged;
    public event FinancialWorkValueChangedHandler<MoneyMovementEntry, decimal>? AmountChanged;
    public event FinancialWorkValueChangedHandler<MoneyMovementEntry, DateTime>? DateChanged;
}
