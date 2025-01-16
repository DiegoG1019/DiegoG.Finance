using MessagePack;
using NodaMoney;

namespace DiegoG.Finance.Serialization;

[MessagePackObject]
public readonly struct MoneyCollectionBuffer
{
    [Key(0)]
    public Currency Currency { get; init; }

    [Key(1)]
    public HashSet<LabeledAmount> AmountSet { get; init; }
}
