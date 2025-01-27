using MessagePack;
using NodaMoney;

namespace DiegoG.Finance.Serialization.JsonConverters;

public readonly struct CategorizedMoneyCollectionBuffer
{
    public Dictionary<string, MoneyCollection> Categories { get; init; }
}
