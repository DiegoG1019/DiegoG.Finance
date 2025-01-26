﻿using NodaMoney;

namespace DiegoG.Finance.Blazor.Services;

public class WorkTable
{
    public WorkTable()
    {
        CurrentSheet = new WorkSheet(Currency.FromCode("COP"));
        var sheet = CurrentSheet.SpendingTrackers;
        sheet.IncomeSources.Add("Day Job", 5000);
        sheet.IncomeSources.Add("Side Gig", 1000);

        var fundamentals = sheet.ExpenseCategories.Add("Fundamentals");
        var fun = sheet.ExpenseCategories.Add("Fun");
        var future = sheet.ExpenseCategories.Add("Future You");
        var algoMas = sheet.ExpenseCategories.Add("Algo mas");
        var valcita = sheet.ExpenseCategories.Add("Valcita");

        fundamentals.Add("Rent", 100);
        fundamentals.Add("Transportation", 200);
        fundamentals.Add("Phone", 300);

        fun.Add("Rappi", 400);
        fun.Add("Going out", 500);

        future.Add("College", 600);
        future.Add("Computer", 700);
        future.Add("Furniture", 800);
        future.Add("Board Games", 900);

        algoMas.Add("Antonio Banderas Fund", 1500);

        valcita.Add("Cremas Antonella", 3000);

        SpendingTrackerSheetControls = new(this);
    }

    public WorkSheet? CurrentSheet
    {
        get => field;
        set
        {
            if (field is not null)
                field.WorkSheetSpendingTrackerSheetMemberChanged -= WorkTable_WorkSheetSpendingTrackerSheetMemberChanged;

            field = value;

            if (value is not null)
                value.WorkSheetSpendingTrackerSheetMemberChanged += WorkTable_WorkSheetSpendingTrackerSheetMemberChanged;

            CurrentSheetChanged?.Invoke();
        }
    }

    private void WorkTable_WorkSheetSpendingTrackerSheetMemberChanged(WorkSheet obj)
    {
        CurrentSheetMemberChanged?.Invoke();
    }

    public SpendingTrackerSheetControls SpendingTrackerSheetControls { get; }

    public event Action? CurrentSheetChanged;
    public event Action? CurrentSheetMemberChanged;

    public void PreAnalyzeSheet()
    {
        SpendingTrackerSheetControls.PreAnalyzeSheet();
    }
}
