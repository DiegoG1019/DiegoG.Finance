using Newtonsoft.Json.Linq;
using NodaMoney;

namespace DiegoG.Finance.Results;

public class SpendingTrackerCategoryResult
{
    internal SpendingTrackerCategoryResult(
        Percentage goal,
        CategorizedMoneyCollection collection,
        ExpenseTypesCollection categorizedMoneyCollection,
        SpendingTrackerSheet sheet
    ) 
    {
        CategorizedMoneyCollection = categorizedMoneyCollection ?? throw new ArgumentNullException(nameof(collection));
        Collection = collection ?? throw new ArgumentNullException(nameof(collection));
        Sheet = sheet ?? throw new ArgumentNullException(nameof(sheet));
        Goal = goal;
    }

    public ExpenseTypesCollection CategorizedMoneyCollection { get; }
    public CategorizedMoneyCollection Collection { get; }
    public SpendingTrackerSheet Sheet { get; }
    public Percentage Goal
    {
        get => field;
        set
        {
            if (field != value)
            {
                var old = field;
                field = value;
                GoalChanged?.Invoke(this, old, value);
            }
        }
    }

    public Percentage ActualPercentage => Percentage.FromRatio(Collection.Total, Sheet.IncomeSources.Total);

    public decimal Total => Collection.Total;

    public event FinancialWorkEventHandler<SpendingTrackerCategoryResult, Percentage>? GoalChanged;
}
