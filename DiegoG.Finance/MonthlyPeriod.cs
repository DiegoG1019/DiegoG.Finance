using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Finance;

[StructLayout(LayoutKind.Explicit)]
[MessagePackObject]
public readonly struct MonthlyPeriod : IEquatable<MonthlyPeriod>, IComparable<MonthlyPeriod>
{
    [FieldOffset(0)]
    internal readonly int RawValue;

    public readonly int ToBinary()
        => RawValue;

    public static MonthlyPeriod FromBinary(int binary)
        => new(binary);

    private MonthlyPeriod(int raw)
    {
        RawValue = raw;
    }

    public MonthlyPeriod(byte Month, short Year)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(Month, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(Month, 12);

        this.Month = Month;
        this.Year = Year;
    }

    [Key(0)]
    [field: FieldOffset(0)]
    public short Month { get; }

    [Key(1)]
    [field: FieldOffset(2)]
    public short Year { get; }

    [IgnoreMember]
    public DateTime Date => new(Year, Month, 1);

    public bool Equals(MonthlyPeriod other)
        => RawValue == other.RawValue;

    public int CompareTo(MonthlyPeriod other)
        => Date.CompareTo(other.Date);

    public override bool Equals(object? obj)
        => obj is MonthlyPeriod other && RawValue == other.RawValue;

    public override int GetHashCode()
        => RawValue.GetHashCode();

    public static bool operator ==(MonthlyPeriod left, MonthlyPeriod right)
        => left.Equals(right);

    public static bool operator !=(MonthlyPeriod left, MonthlyPeriod right)
        => !(left == right);

    public static bool operator <(MonthlyPeriod left, MonthlyPeriod right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(MonthlyPeriod left, MonthlyPeriod right)
        => left.CompareTo(right) <= 0;

    public static bool operator >(MonthlyPeriod left, MonthlyPeriod right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(MonthlyPeriod left, MonthlyPeriod right)
        => left.CompareTo(right) >= 0;
}

