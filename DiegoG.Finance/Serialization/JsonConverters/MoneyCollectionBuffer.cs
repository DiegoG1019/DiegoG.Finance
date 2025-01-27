using MessagePack;
using NodaMoney;

namespace DiegoG.Finance.Serialization.JsonConverters;

public readonly struct MoneyCollectionBuffer
{
    public HashSet<LabeledAmount> AmountSet { get; init; }
}
