using DiegoG.Finance.Internal;
using System.Diagnostics;

namespace DiegoG.Finance;

public sealed class MoneyMovementTotalTracker : FinancialWork<MoneyMovementTotalTracker>
{
    private readonly HashSet<MoneyMovementEntry> Entries = [];

    internal MoneyMovementTotalTracker(WorkSheet sheet) : base(sheet) { }

    internal void Attach(MoneyMovementEntry entry)
    {
        if (Entries.Add(entry))
        {
            if (entry.Amount < 0)
                TotalSpent += -entry.Amount;

            Total += entry.Amount;

            entry.Internal_AmountChanged += Entry_Internal_AmountChanged;
        }
    }

    internal void Detach(MoneyMovementEntry entry)
    {
        if (Entries.Remove(entry))
        {
            if (entry.Amount < 0)
                TotalSpent += entry.Amount;
            Total -= entry.Amount;

            entry.Internal_AmountChanged -= Entry_Internal_AmountChanged;
        }
    }

    private void Entry_Internal_AmountChanged(MoneyMovementEntry sender, decimal oldValue, decimal newValue)
    {
        Debug.Assert(Entries.Contains(sender));
        if (newValue < 0)
        {
            if (oldValue < 0) 
                TotalSpent += oldValue;
            TotalSpent += -newValue;
        }

        Total += newValue - oldValue;
    }

    public decimal Total { get; private set; }
    public decimal TotalSpent { get; private set; }

    internal static MoneyMovementTotalTracker TrackerFactory(Type _, FinancialWork category)
        => new(category.Sheet);
}
