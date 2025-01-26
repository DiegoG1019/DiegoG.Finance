using DiegoG.Finance.Results;
using NodaMoney;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using static DiegoG.Finance.WorkSheet;

namespace DiegoG.Finance;

public class SpendingTrackerSheet : IFinancialWork, IInternalCategorizedMoneyCollectionParent, IInternalMoneyCollectionParent
{
    private readonly ReferredReference<Action<MoneyCollection, LabeledAmount>> internal_MoneyCollectionMemberChanged;
    private readonly ReferredReference<Action<CategorizedMoneyCollection, MoneyCollection>> internal_CategorizedMoneyCollectionMemberChanged;

    internal IInternalSpendingTrackerSheetParent? Parent;

    internal SpendingTrackerSheet(
        Currency? currency,
        MoneyCollection? incomeSources,
        CategorizedMoneyCollection? expenseCategories
    )
    {
        internal_CategorizedMoneyCollectionMemberChanged = new(Handler_internal_CollectionMemberChanged);
        internal_MoneyCollectionMemberChanged = new(Handler_internal_CollectionMemberChanged);

        if (incomeSources is null)
            IncomeSources = new(Currency);
        else
        {
            IncomeSources = incomeSources;
            if (IncomeSources.Parent is not null)
                throw new ArgumentException($"The MoneyCollection for IncomeSources already has a parent");
        }

        IncomeSources.Parent = this;

        if (expenseCategories is null)
            ExpenseCategories = new(Currency);
        else
        {
            ExpenseCategories = expenseCategories;
            if (ExpenseCategories.Parent is not null)
                throw new ArgumentException($"The CategorizedMoneyCollection for ExpenseCategories already has a parent");
        }

        ExpenseCategories.Parent = this;

        Results = new(this);
        ExpenseCategories.CollectionChanged += ExpenseCategories_CollectionChanged;

        Currency = currency ?? default;
    }

    public SpendingTrackerSheet(
        Currency currency
    ) : this(currency, null, null) { }

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

    public Currency Currency => Parent?.Currency ?? field;
    public MoneyCollection IncomeSources { get; } 
    public CategorizedMoneyCollection ExpenseCategories { get; }

    [JsonIgnore]
    public SpendingTrackerSheetResults Results { get; }

    public event Action<SpendingTrackerSheet>? SpendingTrackerSheetMemberChanged;

    private void Handler_internal_CollectionMemberChanged<T1, T2>(T1 _, T2 __)
    {
        Parent?.Internal_MemberChanged?.Value?.Invoke(this);
        SpendingTrackerSheetMemberChanged?.Invoke(this);
    }

    ReferredReference<Action<MoneyCollection, LabeledAmount>> IInternalMoneyCollectionParent.Internal_MemberChanged
        => internal_MoneyCollectionMemberChanged;

    ReferredReference<Action<CategorizedMoneyCollection, MoneyCollection>> IInternalCategorizedMoneyCollectionParent.Internal_MemberChanged
        => internal_CategorizedMoneyCollectionMemberChanged;
}
