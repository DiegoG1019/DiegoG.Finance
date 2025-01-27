using GLV.Shared.Blazor;
using GLV.Shared.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace DiegoG.Finance.Blazor.ModelControls.SpendingTrackerSheet;

public sealed record class SpendingTrackerSheetControls(WorkTable WorkTable)
{
    [ThreadStatic] private static StringBuilder? StyleBuilder;

    public int LongestTable { get; private set; }

    public void AddCategory()
    {
        WorkTable.CurrentSheet?.SpendingTrackers.ExpenseCategories.Add($"New Sheet [{Random.Shared.Next(0, 9999)}]");
    }

    public static string GetStyleVariablesForCategoryResult(string category, out string lighter)
    {
        (StyleBuilder ??= new StringBuilder()).Clear();
        var color = GetColorForCategory(category);

        StyleBuilder
            .Append("--category-color: ").AppendHlsColor(color).Append(';')
            .Append("--category-border-color: ").AppendHlsColor(color.GetHue(), color.GetSaturation(), color.GetBrightness() * 0.5f).Append(';');
        var catcolor = StyleBuilder.ToString();

        StyleBuilder.Clear();
        StyleBuilder.Append("--category-color: ").AppendHlsColor(color.GetHue(), color.GetSaturation(), color.GetBrightness() * 1.7f).Append(';')
                    .Append("--category-border-color: ").AppendHlsColor(color).Append(';');
        lighter = StyleBuilder.ToString();

        return catcolor;
    }

    public static string GetStyleVariablesForCategory(string category, out string light, out string dark)
    {
        const float LightMultiplier = 1.3f * 1.4f;
        const float DarkMultiplier = 1.3f * 1.2f;

        (StyleBuilder ??= new StringBuilder()).Clear();
        var color = GetColorForCategory(category);

        StyleBuilder.Append("--category-color: ").AppendHlsColor(color).Append(';');
        var style = StyleBuilder.ToString();

        StyleBuilder.Clear();
        StyleBuilder.Append("--category-row-color: ")
            .AppendHlsColor(color.GetHue(), color.GetSaturation(), color.GetBrightness() * LightMultiplier).Append(';');
        light = StyleBuilder.ToString();

        StyleBuilder.Clear();
        StyleBuilder.Append("--category-row-color: ")
            .AppendHlsColor(color.GetHue(), color.GetSaturation(), color.GetBrightness() * DarkMultiplier).Append(';');
        dark = StyleBuilder.ToString();

        return style;
    }

    public static Color GetColorForCategory(string category)
    {
        int len = GLVColors.StandardColors.Length;
        uint index = 0;
        category.TryHashToSHA256(MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref index, 1)), true);
        return GLVColors.StandardColors[(int)(index % len)];
    }

    public void PreAnalyzeSheet()
    {
        Debug.Assert(WorkTable?.CurrentSheet?.SpendingTrackers is not null);

        foreach (var cat in WorkTable.CurrentSheet.SpendingTrackers.ExpenseCategories)
            LongestTable = int.Max(cat.Value.Count, LongestTable);
    }

    public IEnumerable<SheetResultInfo> EnumerateSheetResults()
    {
        Debug.Assert(WorkTable?.CurrentSheet?.SpendingTrackers is not null);

        foreach (var cat in WorkTable.CurrentSheet.SpendingTrackers.Results)
        {
            yield return new SheetResultInfo(
                cat.Value,
                GetStyleVariablesForCategoryResult(cat.Key, out var lighter),
                lighter,
                cat.Value.Collection.GetHashCode()
            );
        }
    }

    private static IEnumerable<ExpenseCategoryInfo> GetInnerExpenseCategories(IEnumerator<ExpenseCategoryInfo> enumerator)
    {
        int i = 0;
        do
        {
            yield return enumerator.Current;
        }
        while (i++ < 2 && enumerator.MoveNext()); // If i == 2, then MoveNext is not called and is safe to call outside. If MoveNext is false instead, it'll be called outside and terminated
    }

    private IEnumerator<ExpenseCategoryInfo> GetExpenseCategoryEnumerator(CategorizedMoneyCollection collection)
    {
        foreach (var cat in collection)
        {
            yield return new(
                cat.Value,
                GetStyleVariablesForCategory(cat.Key, out var light, out var dark),
                LongestTable,
                light,
                dark,
                collection,
                cat.Value.GetHashCode()
            );
        }
    }

    public IEnumerable<LabeledAmountInfo> EnumerateAmounts(MoneyCollection collection)
    {
        Debug.Assert(WorkTable?.CurrentSheet?.SpendingTrackers is not null);
        foreach (var cat in collection)
            yield return new LabeledAmountInfo(cat, collection);
    }

    public IEnumerable<IEnumerable<ExpenseCategoryInfo>> EnumerateExpenseCategories()
    {
        Debug.Assert(WorkTable?.CurrentSheet?.SpendingTrackers is not null);

        var enumerator = GetExpenseCategoryEnumerator(WorkTable.CurrentSheet.SpendingTrackers.ExpenseCategories);
        while (enumerator.MoveNext())
            yield return GetInnerExpenseCategories(enumerator);
    }
}
