using DiegoG.Finance.Results;
using NodaMoney;

namespace DiegoG.Finance;

public interface IFinancialWork
{
    public Currency Currency { get; }
}

internal interface IInternalSpendingTrackerSheetResultParent : IFinancialWork
{
    internal ReferredReference<Action<SpendingTrackerCategoryResult, Percentage>> Internal_GoalChanged { get; }
}

internal interface IInternalLabeledAmountParent : IFinancialWork
{
    internal ReferredReference<Action<LabeledAmount, decimal>> Internal_MemberChanged { get; }
}

internal interface IInternalMoneyCollectionParent : IFinancialWork
{
    internal ReferredReference<Action<MoneyCollection, LabeledAmount>> Internal_MemberChanged { get; }
}

internal interface IInternalCategorizedMoneyCollectionParent : IFinancialWork
{
    internal ReferredReference<Action<CategorizedMoneyCollection, MoneyCollection>> Internal_MemberChanged { get; }
}

internal interface IInternalSpendingTrackerSheetParent : IFinancialWork
{
    internal ReferredReference<Action<SpendingTrackerSheet>> Internal_MemberChanged { get; }
}