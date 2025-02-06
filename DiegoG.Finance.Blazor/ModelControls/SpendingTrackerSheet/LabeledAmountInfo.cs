namespace DiegoG.Finance.Blazor.ModelControls.SpendingTrackerSheet;
public sealed record class LabeledAmountInfo(
    LabeledAmount LabeledAmount,
    MoneyCollection Collection
)
{
    public void Delete()
    {
        Collection.Remove(LabeledAmount);
    }

    public string Label
    {
        get => LabeledAmount.Label;
        set => LabeledAmount.Label = value;
    }

    public string Amount
    {
        get => LabeledAmount.Amount.ToString();
        set
        {
            if (decimal.TryParse(value, out var dec)) 
                LabeledAmount.Amount = dec;
        }
    }
}
