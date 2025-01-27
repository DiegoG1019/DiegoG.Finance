namespace DiegoG.Finance.Blazor.ModelControls.SpendingTrackerSheet;

public sealed record class ExpenseCategoryInfo(
    MoneyCollection Money,
    string Style,
    int LongestTable,
    string LightRowColor,
    string DarkRowColor,
    CategorizedMoneyCollection Collection,
    int Id
)
{
    public string Category
    {
        get => Money.Category ?? "?";
        set => Collection.Rename(Money.Category!, value);
    }

    public readonly record struct ExpenseCategoryRow(LabeledAmountInfo? Value, string Style);
    public IEnumerable<ExpenseCategoryRow> GetRows()
    {
        int count = 0;
        foreach (var mon in Money)
            yield return new ExpenseCategoryRow(
                new LabeledAmountInfo(mon, Money),
                count++ % 2 == 0 ? LightRowColor : DarkRowColor
            );

        for (; count < LongestTable; count++)
            yield return new(
                null,
                count % 2 == 0 ? LightRowColor : DarkRowColor
            );
    }
}
