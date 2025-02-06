using NodaMoney;
using System.Diagnostics.CodeAnalysis;

namespace DiegoG.Finance;

public sealed class LabeledAmount(string label, decimal amount) : IFinancialEntry
{
    public string Label
    {
        get => field;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            var old = field;
            field = value;
            LabelChanged?.Invoke(this, old, value);
        }
    } = label;

    public decimal Amount
    {
        get => field;
        set
        {
            var old = field;
            field = value;
            Internal_AmountChanged?.Value?.Invoke(this, old, value);
            AmountChanged?.Invoke(this, old, value);
        }
    } = amount;

    internal ReferredReference<FinancialWorkEventHandler<LabeledAmount, decimal>>? Internal_AmountChanged;
    public event FinancialWorkEventHandler<LabeledAmount, decimal>? AmountChanged;
    public event FinancialWorkEventHandler<LabeledAmount, string>? LabelChanged;
}
