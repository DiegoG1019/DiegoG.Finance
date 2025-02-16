using MessagePack;
using System.Numerics;

namespace DiegoG.Finance;

[MessagePackObject]
public readonly record struct Percentage(decimal ValueOverHundred)
    : IEquatable<Percentage>, 
      IComparable<Percentage>,
      IEqualityOperators<Percentage, Percentage, bool>,
      IEqualityOperators<Percentage, decimal, bool>,
      IComparisonOperators<Percentage, Percentage, bool>,
      IComparisonOperators<Percentage, decimal, bool>
{
    [Key(0)]
    public decimal ValueOverHundred { get; } = ValueOverHundred;

    [IgnoreMember]
    public decimal PercentageValue => ValueOverHundred * 100;

    public override string ToString()
        => ValueOverHundred.ToString("0.##%");

    public static Percentage FromRatio(decimal a, decimal b)
        => new(a / b);

    public static Percentage FromPercentageValue(decimal percentageValue)
        => new(percentageValue / 100);

    public static bool operator ==(Percentage left, decimal right)
        => left.ValueOverHundred == right;

    public static bool operator !=(Percentage left, decimal right)
        => left.ValueOverHundred != right;

    public int CompareTo(Percentage other)
        => ValueOverHundred.CompareTo(other.ValueOverHundred);

    public static bool operator >(Percentage left, Percentage right)
        => left.ValueOverHundred > right.ValueOverHundred;

    public static bool operator >=(Percentage left, Percentage right)
        => left.ValueOverHundred >= right.ValueOverHundred;

    public static bool operator <(Percentage left, Percentage right)
        => left.ValueOverHundred < right.ValueOverHundred;

    public static bool operator <=(Percentage left, Percentage right)
        => left.ValueOverHundred <= right.ValueOverHundred;

    public static bool operator >(Percentage left, decimal right)
        => left.ValueOverHundred > right;

    public static bool operator >=(Percentage left, decimal right)
        => left.ValueOverHundred >= right;

    public static bool operator <(Percentage left, decimal right)
        => left.ValueOverHundred < right;

    public static bool operator <=(Percentage left, decimal right)
        => left.ValueOverHundred <= right;
}
