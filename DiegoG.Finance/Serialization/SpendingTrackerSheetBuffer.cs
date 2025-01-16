using MessagePack;
using NodaMoney;

namespace DiegoG.Finance.Serialization;

[MessagePackObject]
public readonly record struct SpendingTrackerSheetBuffer
{
    [Key(0)]
    public Currency Currency { get; init; }

    [Key(1)]
    public CategorizedMoneyCollection ExpenseCategories { get; init; }

    [Key(2)]
    public MoneyCollection IncomeSources { get; init; }
}
