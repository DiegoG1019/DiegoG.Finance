using System.Runtime.InteropServices;

namespace DiegoG.Finance;

#if false

[StructLayout(LayoutKind.Explicit)]
public readonly struct MonthlySpan : IEquatable<MonthlySpan>, IComparable<MonthlySpan>
{
    [FieldOffset(0)]
    private readonly long RawValue;

    public MonthlySpan(byte StartMonth, short StartYear, byte EndMonth, short EndYear)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(StartMonth, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(EndMonth, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(StartMonth, 12);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(EndMonth, 12);
        
        if (StartYear >= EndYear)
            throw new ArgumentException("Parameter cannot be less than or equal to StartYear", nameof(EndYear));

        this.StartMonth = StartMonth;
        this.StartYear = StartYear;
        this.EndMonth = EndMonth;
        this.EndYear = EndYear;
    }

    [field: FieldOffset(0)]
    public short StartMonth { get; }

    [field: FieldOffset(2)]
    public short StartYear { get; }

    [field: FieldOffset(4)]
    public short EndMonth { get; }

    [field: FieldOffset(6)]
    public short EndYear { get; }

    public DateTime StartDate => new(StartYear, StartMonth, 1);
    public DateTime EndDate => new(EndYear, EndMonth, 1);

    public static bool AnyOverlaps(MonthlySpan period, IEnumerable<MonthlySpan> periods)
    {
        foreach (var other in periods)
            if (period.StartDate >= other.StartDate && period.StartDate <= other.EndDate
                || period.EndDate >= other.StartDate && period.EndDate <= other.EndDate
                || period.StartDate <= other.StartDate && period.EndDate >= other.EndDate)
                return true;
        return false;
    }

    public bool Equals(MonthlySpan other)
        => RawValue == other.RawValue;

    public int CompareTo(MonthlySpan other)
        => StartDate.CompareTo(other.StartDate);

    public override bool Equals(object? obj)
        => obj is MonthlySpan other && RawValue == other.RawValue;

    public override int GetHashCode()
        => RawValue.GetHashCode();

    public static bool operator ==(MonthlySpan left, MonthlySpan right) 
        => left.Equals(right);

    public static bool operator !=(MonthlySpan left, MonthlySpan right) 
        => !(left == right);

    public static bool operator <(MonthlySpan left, MonthlySpan right) 
        => left.CompareTo(right) < 0;

    public static bool operator <=(MonthlySpan left, MonthlySpan right) 
        => left.CompareTo(right) <= 0;

    public static bool operator >(MonthlySpan left, MonthlySpan right) 
        => left.CompareTo(right) > 0;

    public static bool operator >=(MonthlySpan left, MonthlySpan right) 
        => left.CompareTo(right) >= 0;
}

#endif