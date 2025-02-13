using DiegoG.Finance.Internal;
using System.Diagnostics;

namespace DiegoG.Finance.Results;

public sealed class SpendingTrackerTypeResult : FinancialEntity<SpendingTrackerSheet, SpendingTrackerTypeResult>
{
    internal SpendingTrackerTypeResult(SpendingTrackerSheet parent, ExpenseType category) : base(parent)
    {
        Debug.Assert(category is not null);
        ExpenseType = category;
    }

    public Percentage Goal { get; set; }

    public Percentage Actual => Percentage.FromRatio(Total, Parent.IncomeSources.Total);

    public decimal Total { get; internal set; }

    public ExpenseType ExpenseType { get; }
}
