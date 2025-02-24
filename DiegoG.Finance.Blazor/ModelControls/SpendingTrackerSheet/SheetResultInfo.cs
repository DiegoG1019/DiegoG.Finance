﻿using DiegoG.Finance.Results;

namespace DiegoG.Finance.Blazor.ModelControls.SpendingTrackerSheet;

public sealed record class SheetResultInfo(
    SpendingTrackerCategoryResult Result,
    string Style,
    string Lighter,
    int Id
)
{
    public void DeleteCategory()
        => Result.CategorizedMoneyCollection.Remove(Result.Collection.ExpenseType!, out _);

    public string Category
    {
        get => Result.Collection.ExpenseType!;
        set => Result.CategorizedMoneyCollection.Rename(Result.Collection.ExpenseType!, value);
    }

    public string Goal
    {
        get => Result.Goal.ToString();
        set
        {
            if (decimal.TryParse(value[^1] == '%' ? value.AsSpan()[..^1] : value, out var dec))
                Result.Goal = Percentage.FromPercentageValue(dec);
        }
    }
}
