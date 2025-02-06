namespace DiegoG.Finance.Blazor.ModelControls.SpendingTrackerSheet;

public sealed record class ExpenseTypeCollectionInfo(
    SpendingTrackerSheetControls Controls,
    CategorizedMoneyCollection Money,
    string Style,
    int LongestTable,
    string LightRowColor,
    string DarkRowColor,
    ExpenseTypesCollection Collection,
    int Id
)
{
    public string ExpenseType
    {
        get => Money.ExpenseType ?? "?";
        set => Collection.Rename(Money.ExpenseType!, value);
    }

    public void AddExpense()
    {
        Money.Add($"Expense: #{Money.Count}", 0);
        Controls.PreAnalyzeSheet();
    }

    public readonly record struct ExpenseExpenseTypeRow(ExpenseCategoryInfo? Value, string Style);
    public IEnumerable<ExpenseExpenseTypeRow> GetRows()
    {
        int count = 0;
        foreach (var mon in Money)
            yield return new ExpenseExpenseTypeRow(
                new ExpenseCategoryInfo(mon, Money),
                count++ % 2 == 0 ? LightRowColor : DarkRowColor
            );

        for (; count < LongestTable; count++)
            yield return new(
                null,
                count % 2 == 0 ? LightRowColor : DarkRowColor
            );
    }
}
