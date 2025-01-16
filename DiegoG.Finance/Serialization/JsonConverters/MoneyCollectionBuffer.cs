using MessagePack;
using NodaMoney;

namespace DiegoG.Finance.Serialization.JsonConverters;

public readonly struct MoneyCollectionBuffer
{
    public Currency Currency { get; init; }

    public HashSet<LabeledAmount> AmountSet { get; init; }
}
