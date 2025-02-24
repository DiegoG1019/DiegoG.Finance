﻿using GLV.Shared.Blazor;
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
        WorkTable.CurrentSheet?.SpendingTracker.ExpenseCategories.Add($"New Sheet [{Random.Shared.Next(0, 9999)}]");
    }

    public void AddIncomeSource()
    {
        var sources = WorkTable.CurrentSheet?.SpendingTracker.IncomeSources;
        sources?.Add($"Source #{sources.Count + 1}", 0);
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
        Debug.Assert(WorkTable?.CurrentSheet?.SpendingTracker is not null);

        foreach (var cat in WorkTable.CurrentSheet.SpendingTracker.ExpenseCategories)
            LongestTable = int.Max(cat.Value.Count, LongestTable);
    }

    public IEnumerable<SheetResultInfo> EnumerateSheetResults()
    {
        Debug.Assert(WorkTable?.CurrentSheet?.SpendingTracker is not null);

        foreach (var cat in WorkTable.CurrentSheet.SpendingTracker.Results)
        {
            yield return new SheetResultInfo(
                cat.Value,
                GetStyleVariablesForCategoryResult(cat.Key, out var lighter),
                lighter,
                cat.Value.Collection.GetHashCode()
            );
        }
    }

    private static IEnumerable<ExpenseTypeCollectionInfo> GetInnerExpenseCategories(IEnumerator<ExpenseTypeCollectionInfo> enumerator)
    {
        int i = 0;
        do
        {
            yield return enumerator.Current;
        }
        while (i++ < 1 && enumerator.MoveNext()); // If i == 1, then MoveNext is not called and is safe to call outside. If MoveNext is false instead, it'll be called outside and terminated
    }

    private IEnumerator<ExpenseTypeCollectionInfo> GetExpenseCategoryEnumerator(ExpenseTypesCollection collection)
    {
        foreach (var cat in collection)
        {
            yield return new(
                this,
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

    public IEnumerable<ExpenseCategoryInfo> EnumerateAmounts(CategorizedMoneyCollection collection)
    {
        Debug.Assert(WorkTable?.CurrentSheet?.SpendingTracker is not null);
        foreach (var cat in collection)
            yield return new ExpenseCategoryInfo(cat, collection);
    }

    public IEnumerable<IEnumerable<ExpenseTypeCollectionInfo>> EnumerateExpenseCategories()
    {
        Debug.Assert(WorkTable?.CurrentSheet?.SpendingTracker is not null);

        var enumerator = GetExpenseCategoryEnumerator(WorkTable.CurrentSheet.SpendingTracker.ExpenseCategories);
        while (enumerator.MoveNext())
            yield return GetInnerExpenseCategories(enumerator);
    }
}
