using System.Diagnostics;

namespace DiegoG.Finance.Internal;

public interface IPagedEntity
{
    internal WorkSheetPage Page { get; }
}

public class PagedEntity<TParent, TSelf> : FinancialEntity<TParent, TSelf>, IPagedEntity
    where TParent : FinancialWork<TParent>, IPagedEntity
    where TSelf : FinancialEntity<TParent, TSelf>
{
    internal PagedEntity(TParent parent) : base(parent)
    {
        Page = parent.Page;
    }

    public WorkSheetPage Page { get; } 
}

public class FinancialEntity<TParent, TSelf> : FinancialWork<TSelf>
    where TParent : FinancialWork<TParent>
    where TSelf : FinancialEntity<TParent, TSelf>
{
    internal FinancialEntity(TParent parent) : base(parent.Sheet)
    {
        Debug.Assert(parent is not null);
        Parent = parent;
    }

    public TParent Parent { get; protected set; }

    public bool IsValid => Parent is not null;

    internal void Invalidate()
    {
        Parent = null!;
        OnInvalidate();
        Internal_ExpenseTypeInvalidated?.Invoke((TSelf)this);
        ExpenseTypeInvalidated?.Invoke((TSelf)this);
    }

    protected virtual void OnInvalidate() { }

    public event FinancialWorkInvalidatedHandler<TParent, TSelf>? ExpenseTypeInvalidated;
    internal event FinancialWorkInvalidatedHandler<TParent, TSelf>? Internal_ExpenseTypeInvalidated;
}
