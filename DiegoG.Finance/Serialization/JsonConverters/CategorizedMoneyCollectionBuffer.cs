using MessagePack;
using NodaMoney;

namespace DiegoG.Finance.Serialization.JsonConverters;

public readonly struct CategorizedMoneyCollectionBuffer
{
    public Currency Currency { get; init; }

    public Dictionary<string, MoneyCollection> Categories { get; init; }
}
