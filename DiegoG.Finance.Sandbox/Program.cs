using MessagePack;
using NodaMoney;
using System.Diagnostics;

namespace DiegoG.Finance.Sandbox;

internal class Program
{
    static void Main(string[] args)
    {
        var sheet = new SpendingTrackerSheet(Currency.FromCode("COP"));

        sheet.IncomeSources.Add(new LabeledAmount("Day Job", 5000000));
        sheet.IncomeSources.Add(new LabeledAmount("Freelancing", 1500000));

        var fun = sheet.ExpenseCategories.Add("Fun");
        fun.Add(new LabeledAmount("Rappi", 10000));
        fun.Add(new LabeledAmount("Clothes", 150_000));

        var fundamentals = sheet.ExpenseCategories.Add("Fundamentals");
        fundamentals.Add(new LabeledAmount("Rent", 2_000_000));
        fundamentals.Add(new LabeledAmount("Phone", 500_000));

        var data = MessagePackSerializer.Serialize(sheet);

        var newSheet = MessagePackSerializer.Deserialize<SpendingTrackerSheet>(data);

        Debugger.Break();
    }
}
