using DiegoG.Finance.Blazor.ModelControls.SpendingTrackerSheet;
using NodaMoney;

namespace DiegoG.Finance.Blazor.ModelControls;

public class WorkTable
{
    public WorkTable()
    {
        SpendingTrackerSheetControls = new(this);
    }

    public WorkSheet? CurrentSheet
    {
        get => field;
        set
        {
            field = value;
            CurrentSheetChanged?.Invoke();
        }
    }

    public SpendingTrackerSheetControls SpendingTrackerSheetControls { get; }

    public event Action? CurrentSheetChanged;

    public void PreAnalyzeSheet()
    {
        SpendingTrackerSheetControls.PreAnalyzeSheet();
    }

    public static WorkSheet NewDefaultWorkSheet()
    {
        var worksheet = new WorkSheet(Currency.FromCode("COP"));
        var tracker = worksheet.SpendingTracker;

        worksheet.ExpenseTypesAndCategories.Income.Add("Day Job", out var dayjob);
        worksheet.ExpenseTypesAndCategories.Income.Add("Side Gig", out var sidegig);

        tracker.IncomeSources.Add(dayjob!).Amount = 4300000;
        tracker.IncomeSources.Add(sidegig!).Amount = 1000;

        worksheet.ExpenseTypesAndCategories.Add("Fundamentals", out var fundamentals);
        worksheet.ExpenseTypesAndCategories.Add("Fun", out var fun);
        worksheet.ExpenseTypesAndCategories.Add("Future", out var future);

        fundamentals!.Add("Rent", out var rent);
        fundamentals.Add("Transportation", out var trans);
        fundamentals.Add("Phone", out var phone);

        fun!.Add("Going out", out var gout);

        future!.Add("College", out var coolleg);
        future.Add("Computer", out var computer);
        future.Add("Furniture", out var furniture);
        future.Add("Board Games", out var games);

        tracker.Expenses.Add(rent!).Amount = 500;
        tracker.Expenses.Add(trans!).Amount = 500;
        tracker.Expenses.Add(phone!).Amount = 500;

        tracker.Expenses.Add(gout!).Amount = 500;

        tracker.Expenses.Add(coolleg!).Amount = 500;
        tracker.Expenses.Add(computer!).Amount = 500;
        tracker.Expenses.Add(furniture!).Amount = 500;
        tracker.Expenses.Add(games!).Amount = 500;

        return worksheet;
    }
}
