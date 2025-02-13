using DiegoG.Finance.Internal;
using DiegoG.Finance.Results;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace DiegoG.Finance;

public class SpendingTrackerSheet : FinancialEntity<WorkSheetPage, SpendingTrackerSheet>
{
    public readonly record struct Info();

    internal SpendingTrackerSheet(
        WorkSheetPage page,
        IEnumerable<SpendingTrackerEntry.Info>? incomeEntries, 
        IEnumerable<SpendingTrackerEntry.Info>? expenseEntries
    ) : base(page)
    {
        IncomeSources = new SpendingTrackerIncomeCollection(this, incomeEntries);
        Expenses = new SpendingTrackerCollection(this, expenseEntries);
    }

    public SpendingTrackerIncomeCollection IncomeSources { get; } 
    public SpendingTrackerCollection Expenses { get; }

    public SpendingTrackerCategoryResult GetExpensesResults(ExpenseCategory category)
    {
        ArgumentNullException.ThrowIfNull(category);
        category.ThrowIfNotSameSheet(Sheet);
        return category.GetOrAddTracker<SpendingTrackerCategoryResult>(cat => new(this, cat));
    }

    [JsonIgnore]
    public Percentage RemainingPercentage => Percentage.FromRatio(Remaining, IncomeSources.Total);

    [JsonIgnore]
    public decimal Remaining => IncomeSources.Total - Expenses.Total;
}
