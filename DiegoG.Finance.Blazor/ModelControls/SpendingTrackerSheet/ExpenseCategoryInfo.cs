namespace DiegoG.Finance.Blazor.ModelControls.SpendingTrackerSheet;

public sealed record class ExpenseCategoryInfo(
    ExpenseCategory ExpenseCategory,
    CategorizedMoneyCollection Collection
)
{
    public void Delete()
    {
        Collection.Remove(ExpenseCategory);
    }

    public string Label
    {
        get => ExpenseCategory.Label;
        set
        {
            if (string.IsNullOrWhiteSpace(value) is false)
                ExpenseCategory.TryRename(value);
        }
    }

    public string Amount
    {
        get => ExpenseCategory.Amount.ToString();
        set
        {
            if (decimal.TryParse(value, out var dec))
                ExpenseCategory.Amount = dec;
        }
    }
}
