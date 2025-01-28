using MessagePack;
using NodaMoney;
using System.Diagnostics;

namespace DiegoG.Finance.Sandbox;

internal class Program
{
    static void Main(string[] args)
    {
        var work = new WorkSheet(Currency.FromCode("COP"));
        var sheet = work.SpendingTrackers;

        sheet.IncomeSources.Add("Day Job", 5000000);
        sheet.IncomeSources.Add("Freelancing", 1500000);

        var fun = sheet.ExpenseCategories.Add("Fun");
        fun.Add("Rappi", 10000);
        fun.Add("Clothes", 150_000);

        var fundamentals = sheet.ExpenseCategories.Add("Fundamentals");
        var rent = fundamentals.Add("Rent", 2_000_000);
        fundamentals.Add("Phone", 500_000);

        Debugger.Break();
        sheet.ExpenseCategories.Rename("Fun", "Fun't!");
        Debugger.Break();

        var data = MessagePackSerializer.Serialize(sheet);

        var newSheet = MessagePackSerializer.Deserialize<SpendingTrackerSheet>(data);

        Debugger.Break();
    }

    private static void Work_WorkSheetSpendingTrackerSheetMemberChanged(WorkSheet obj)
    {
        Console.WriteLine("Wa0s!");
    }
}
