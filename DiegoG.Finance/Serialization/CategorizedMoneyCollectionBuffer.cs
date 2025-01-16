using MessagePack;
using NodaMoney;

namespace DiegoG.Finance.Serialization;

[MessagePackObject]
public readonly struct CategorizedMoneyCollectionBuffer
{
    [Key(0)]
    public Currency Currency { get; init; }

    [Key(1)]
    public Dictionary<string, MoneyCollection> Categories { get; init; }
}
