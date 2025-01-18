using MessagePack;

namespace DiegoG.Finance;

public readonly struct WorkSheetHeader
{
    public required bool IsNotEmpty { get; init; }

    public required string Name { get; init; }

    public required bool IsPasswordProtected { get; init; }

    public required DateTimeOffset Created { get; init; }

    public required int Version { get; init; }
    
    public string? Path { get; init; }
}
