using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace DiegoG.Finance.Internal;

public class FinancialWork<TSelf> : FinancialWork
    where TSelf : FinancialWork<TSelf>
{
    internal FinancialWork(WorkSheet sheet) : base(sheet) { }

    [StackTraceHidden]
    internal void ThrowIfNotSameSheet(FinancialWork other)
        => ThrowIfNotSameSheet(other.Sheet);

    [StackTraceHidden]
    internal void ThrowIfNotSameSheet(WorkSheet other)
    {
        if (Sheet != other)
            throw new ArgumentException("Cannot attach an entity from a different WorkSheet");
    }

    internal TValue? ChangeValue<TValue>(
        [NotNullIfNotNull(nameof(value))] ref TValue? field,
        in TValue value,
        FinancialWorkValueChangedHandler<TSelf, TValue>? internal_event,
        FinancialWorkValueChangedHandler<TSelf, TValue>? _event
    )
    {
        var old = field;
        field = value;
        internal_event?.Invoke((TSelf)this, old, value);
        _event?.Invoke((TSelf)this, old, value);
        return old;
    }
}

public class FinancialWork
{
    public WorkSheet Sheet { get; }

    internal FinancialWork(WorkSheet sheet)
    {
        Debug.Assert(sheet is not null);
        Sheet = sheet;
    }
}
