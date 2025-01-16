using MessagePack;
using NodaMoney;

namespace DiegoG.Finance.Serialization.JsonConverters;

public readonly record struct SpendingTrackerSheetBuffer
{
    public Currency Currency { get; init; }

    public Dictionary<string, MoneyCollection> ExpenseCategories { get; init; }

    public HashSet<LabeledAmount> IncomeSources { get; init; }
}
