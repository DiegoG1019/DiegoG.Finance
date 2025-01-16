using NodaMoney;
using System.Diagnostics.CodeAnalysis;

namespace DiegoG.Finance;

public readonly record struct LabeledMoney(string? Label, Money Money)
{
    public static implicit operator KeyValuePair<string?, Money>(LabeledMoney label)
        => new(label.Label, label.Money);

    public sealed class LabeledMoneyComparer : IEqualityComparer<LabeledMoney>
    {
        private LabeledMoneyComparer() { }
        public static LabeledMoneyComparer Instance { get; } = new();

        public bool Equals(LabeledMoney x, LabeledMoney y)
            => x.Label == y.Label;

        public int GetHashCode([DisallowNull] LabeledMoney obj)
            => HashCode.Combine(obj.Label?.GetHashCode() ?? 0, obj.Money.Currency);
    }
}

public readonly record struct LabeledAmount(string? Label, decimal Amount)
{
    public static implicit operator KeyValuePair<string?, decimal>(LabeledAmount label)
        => new(label.Label, label.Amount);

    public sealed class LabeledAmountComparer : IEqualityComparer<LabeledAmount>
    {
        private LabeledAmountComparer() { }
        public static LabeledAmountComparer Instance { get; } = new();

        public bool Equals(LabeledAmount x, LabeledAmount y)
            => x.Label == y.Label;

        public int GetHashCode([DisallowNull] LabeledAmount obj)
            => obj.Label?.GetHashCode() ?? 0;
    }
}
