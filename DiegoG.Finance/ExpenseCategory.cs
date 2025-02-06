using System.Diagnostics;

namespace DiegoG.Finance;

public sealed class ExpenseCategory : IFinancialEntry
{
    internal ExpenseCategory(string label, decimal amount, CategorizedMoneyCollection moneyCollection)
    {
        Debug.Assert(string.IsNullOrWhiteSpace(label) is false);
        Debug.Assert(moneyCollection is not null);

        Label = label;
        Amount = amount;
        Collection = moneyCollection;
    }

    public CategorizedMoneyCollection Collection { get; }

    public string Label 
    { 
        get; 
        internal set
        {
            var old = field;
            field = value;
            LabelChanged?.Invoke(this, old, value);
        }
    }

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
    }

    public bool TryRename(string newLabel)
        => Collection.RenameCategory(Label, newLabel);

    internal ReferredReference<FinancialWorkEventHandler<ExpenseCategory, decimal>>? Internal_AmountChanged;
    public event FinancialWorkEventHandler<ExpenseCategory, decimal>? AmountChanged;
    public event FinancialWorkEventHandler<ExpenseCategory, string>? LabelChanged;
}
