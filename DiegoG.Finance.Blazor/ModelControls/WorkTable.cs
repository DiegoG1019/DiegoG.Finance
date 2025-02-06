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
        var sheet = worksheet.SpendingTrackers;
        sheet.IncomeSources.Add("Day Job", 5000);
        sheet.IncomeSources.Add("Side Gig", 1000);

        var fundamentals = sheet.ExpenseCategories.Add("Fundamentals");
        var fun = sheet.ExpenseCategories.Add("Fun");
        var future = sheet.ExpenseCategories.Add("Future You");

        fundamentals.Add("Rent", 100);
        fundamentals.Add("Transportation", 200);
        fundamentals.Add("Phone", 300);

        fun.Add("Going out", 500);

        future.Add("College", 600);
        future.Add("Computer", 700);
        future.Add("Furniture", 800);
        future.Add("Board Games", 900);

        return worksheet;
    }
}
