using NodaMoney;
using System.Diagnostics.CodeAnalysis;

namespace DiegoG.Finance;

public sealed class LabeledAmount(string label, decimal amount)
{
    internal LabeledAmount(IInternalLabeledAmountParent internalMoneyCollection, string label, decimal amount) : this(label, amount)
    {
        Parent = internalMoneyCollection;
    }

    internal IInternalLabeledAmountParent? Parent;

    public string Label
    {
        get => field;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (field != value)
            {
                Parent?.Internal_MemberChanged.Value.Invoke(this, 0);
                field = value;
            }
        }
    } = label;

    public decimal Amount
    {
        get => field;
        set
        {

            if (field != value)
            {
                Parent?.Internal_MemberChanged.Value.Invoke(this, field - value);
                field = value;
            }
        }
    } = amount;

    public Currency? Currency => Parent?.Currency;

    public bool TryGetMoney([MaybeNullWhen(false)] out Money money)
    {
        if (Currency is Currency cur)
        {
            money = new(Amount, cur);
            return true;
        }

        money = default;
        return false;
    }

    public static implicit operator KeyValuePair<string, decimal>(LabeledAmount label)
        => new(label.Label, label.Amount);
}
