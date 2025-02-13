using DiegoG.Finance.Internal;
using MessagePack;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace DiegoG.Finance;

public sealed class WorkSheetPage : FinancialEntity<WorkSheetBook, WorkSheetPage>, IPagedEntity
{
    [MessagePackObject]
    public readonly record struct Info(
        [property: Key(0)] IEnumerable<SpendingTrackerEntry.Info>? IncomeEntries, 
        [property: Key(1)] IEnumerable<SpendingTrackerEntry.Info>? ExpenseEntries,
        [property: Key(2)] MoneyMovementTracker.Info? Movements
    );

    internal Info GetInfo()
        => new(
            SpendingTracker.IncomeSources.Select(x => new SpendingTrackerEntry.Info(x.Category.Parent.Name, x.Category.Name, x.Amount)),
            SpendingTracker.Expenses.Select(x => new SpendingTrackerEntry.Info(x.Category.Parent.Name, x.Category.Name, x.Amount)),
            MoneyTracker.GetInfo()
        );

    internal WorkSheetPage(MonthlyPeriod period, WorkSheetBook book, Info? info) : base(book)
    {
        Period = period;
        if (info is Info i)
        {
            SpendingTracker = new(this, i.IncomeEntries, i.ExpenseEntries);
            MoneyTracker = new(this, i.Movements);
        }
        else
        {
            SpendingTracker = new(this, null, null);
            MoneyTracker = new(this, null);
        }
    }

    public SpendingTrackerSheet SpendingTracker
    {
        get
        {
            Debug.Assert(field is not null);
            return field;
        }
    }

    public MoneyMovementTracker MoneyTracker
    {
        get 
        {
            Debug.Assert(field is not null);
            return field;
        }
    }

    public MonthlyPeriod Period { get; }

    // -- Internal data tracking

    // - Expense Category

    internal bool TryGetCategoryTracker<T>(ExpenseCategory category, [NotNullWhen(true)] out T? value) where T : FinancialWork
    {
        Debug.Assert(category is not null);
        var info = ExpenseCategoryInfo.GetOrAdd(category, cat => new());
        if (info.Trackers.TryGetValue(typeof(T), out var v))
        {
            value = (T)v;
            return true;
        }

        value = null;
        return false;
    }

    internal T GetOrAddCategoryTracker<T>(ExpenseCategory category, Func<Type, ExpenseCategory, FinancialWork> entityFactory) where T : FinancialWork
    {
        Debug.Assert(category is not null);
        Debug.Assert(entityFactory is not null);

        var info = ExpenseCategoryInfo.GetOrAdd(category, cat => new());
        return (T)info.Trackers.GetOrAdd(typeof(T), entityFactory, category);
    }

    internal bool DetachCategoryTracker<T>(ExpenseCategory category) where T : FinancialWork
    {
        Debug.Assert(category is not null);
        return ExpenseCategoryInfo.TryGetValue(category, out var info) && info.Trackers.TryRemove(typeof(T), out _);
    }

    // - Expense Type

    internal bool TryGetTypeTracker<T>(ExpenseType exptype, [NotNullWhen(true)] out T? value) where T : FinancialWork
    {
        Debug.Assert(exptype is not null);
        var info = ExpenseTypeInfo.GetOrAdd(exptype, cat => new());
        if (info.Trackers.TryGetValue(typeof(T), out var v))
        {
            value = (T)v;
            return true;
        }

        value = null;
        return false;
    }
    internal T GetOrAddTypeTracker<T>(ExpenseType exptype, Func<Type, ExpenseType, FinancialWork> entityFactory) where T : FinancialWork
    {
        Debug.Assert(exptype is not null);
        Debug.Assert(entityFactory is not null);

        var info = ExpenseTypeInfo.GetOrAdd(exptype, cat => new());
        return (T)info.Trackers.GetOrAdd(typeof(T), entityFactory, exptype);
    }

    internal bool DetachTypeTracker<T>(ExpenseType exptype) where T : FinancialWork
    {
        Debug.Assert(exptype is not null);
        return ExpenseTypeInfo.TryGetValue(exptype, out var info) && info.Trackers.TryRemove(typeof(T), out _);
    }

    internal bool AttachEntity<T>(ExpenseCategory category, T entity) where T : FinancialWork
    {
        Debug.Assert(category is not null);
        Debug.Assert(entity is not null);
        var info = ExpenseCategoryInfo.GetOrAdd(category, cat => new());
        var coll = info.EntitiesByType.GetOrAdd(typeof(T), t => []);
        return coll.Add(entity);
    }

    // - Entitites

    internal bool DetachEntity<T>(ExpenseCategory category, T entity) where T : FinancialWork
    {
        Debug.Assert(category is not null);
        return ExpenseCategoryInfo.TryGetValue(category, out var info) && info.EntitiesByType.TryGetValue(typeof(T), out var coll) && coll.Remove(entity);
    }

    public IReadOnlyCollection<T> GetAttachedEntities<T>(ExpenseCategory category) where T : FinancialWork
    {
        Debug.Assert(category is not null);
        ThrowIfNotSameSheet(category.Sheet);
        return ExpenseCategoryInfo.TryGetValue(category, out var info) && info.EntitiesByType.TryGetValue(typeof(T), out var coll)
                ? new CastCollectionWrappers<T, FinancialWork>(coll)
                : Array.Empty<T>();
    }

    public IReadOnlyCollection<T> GetAttachedEntities<T>(ExpenseType exptype) where T : FinancialWork
    {
        Debug.Assert(exptype is not null);
        ThrowIfNotSameSheet(exptype.Sheet);
        return new ExpenseTypeEntityCollection<T>(exptype, ExpenseCategoryInfo);
    }

    private sealed class ExpenseTypeEntityCollection<T> : IReadOnlyCollection<T> where T : FinancialWork
    {
        public int Count { get; }
        public ExpenseType ExpenseType { get; }
        public ConcurrentDictionary<ExpenseCategory, CategoryInfoCell> ExpenseCategoryInfo { get; }

        public ExpenseTypeEntityCollection(
            ExpenseType exptype, 
            ConcurrentDictionary<ExpenseCategory, CategoryInfoCell> expenseCategoryInfo
    )
        {
            ExpenseType = exptype;
            ExpenseCategoryInfo = expenseCategoryInfo;
            Count = exptype._categories.Values.Sum(GetCount);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var category in ExpenseType._categories.Values)
            {
                if (ExpenseCategoryInfo.TryGetValue(category, out var info) && info.EntitiesByType.TryGetValue(typeof(T), out var coll))
                    foreach (var v in coll)
                        yield return (T)v;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private int GetCount(ExpenseCategory category)
            => ExpenseCategoryInfo.TryGetValue(category, out var info) && info.EntitiesByType.TryGetValue(typeof(T), out var coll)
                ? coll.Count
                : 0;
    }

    // ---- Internal data tracking info collections

    private sealed class CategoryInfoCell()
    {
        public readonly ConcurrentDictionary<Type, WeakReferenceSetCollection<FinancialWork>> EntitiesByType = [];
        public readonly ConcurrentDictionary<Type, FinancialWork> Trackers = [];
    }

    private sealed class TypeInfoCell()
    {
        public readonly ConcurrentDictionary<Type, FinancialWork> Trackers = [];
    }

    private readonly ConcurrentDictionary<ExpenseCategory, CategoryInfoCell> ExpenseCategoryInfo = [];
    private readonly ConcurrentDictionary<ExpenseType, TypeInfoCell> ExpenseTypeInfo = [];

    WorkSheetPage IPagedEntity.Page => this;
}
