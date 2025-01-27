using DiegoG.Finance.Results;
using NodaMoney;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using static DiegoG.Finance.WorkSheet;

namespace DiegoG.Finance;

public class SpendingTrackerSheet
{
    internal SpendingTrackerSheet(
        MoneyCollection? incomeSources,
        CategorizedMoneyCollection? expenseCategories
    )
    {
        IncomeSources = incomeSources is null ? [] : incomeSources;
        ExpenseCategories = expenseCategories is null ? ([]) : expenseCategories;

        Results = new(this);
        ExpenseCategories.Internal_CollectionChanged = new(ExpenseCategories_CollectionChanged);
    }

    public SpendingTrackerSheet() : this(null, null) { }

    private void ExpenseCategories_CollectionChanged(
        CategorizedMoneyCollection arg1, 
        NotifyCollectionChangedAction arg2, 
        KeyValuePair<string, MoneyCollection> arg3
    )
    {
        if (arg2 is NotifyCollectionChangedAction.Add)
        {
            Results.Add(arg3.Key, default, arg3.Value, arg1);
        }    
        else if (arg2 is NotifyCollectionChangedAction.Remove)
        {
            Results.Remove(arg3.Key);
        }
        else if (arg2 is NotifyCollectionChangedAction.Reset)
        {
            Results.Clear();
            foreach (var item in arg1)
                Results.Add(item.Key, new Percentage(0), item.Value, arg1);
        }
    }

    public MoneyCollection IncomeSources { get; } 
    public CategorizedMoneyCollection ExpenseCategories { get; }

    [JsonIgnore]
    public SpendingTrackerSheetResults Results { get; }
}
