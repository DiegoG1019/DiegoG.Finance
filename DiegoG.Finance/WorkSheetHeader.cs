using MessagePack;
using NodaMoney;

namespace DiegoG.Finance;

[MessagePackObject]
public readonly struct WorkSheetHeader
{
    [IgnoreMember]
    public required bool IsNotEmpty { get; init; }

    [Key(4)]
    public required string Name { get; init; }

    [Key(2)]
    public required DateTimeOffset Created { get; init; }

    [Key(0)]
    public required int Version { get; init; }

    [Key(1)]
    public required int Pages { get; init; }

    [Key(3)]
    public required Currency Currency { get; init; }

    [IgnoreMember]
    public string? Path { get; init; }
}
