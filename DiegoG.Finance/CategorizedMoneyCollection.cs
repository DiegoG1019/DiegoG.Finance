using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security;

namespace DiegoG.Finance;

public class CategorizedMoneyCollection
    : MoneyCollectionBase<CategorizedMoneyCollection, ExpenseCategory, Dictionary<string, ExpenseCategory>, KeyValuePair<string, ExpenseCategory>>,
      IReadOnlyCollection<ExpenseCategory>
{
    internal ReferredReference<FinancialWorkEventHandler<ExpenseCategory, decimal>> Internal_Handler_AmountChanged;
    internal ReferredReference<MoneyCollectionTotalChangedEventHandler<CategorizedMoneyCollection, ExpenseCategory, Dictionary<string, ExpenseCategory>, KeyValuePair<string, ExpenseCategory>>>? Internal_TotalChanged;

    internal FinancialCategoryCollection? CategoryInfo
    {
        get => field;
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value.AddExpenseType(ExpenseType) is false)
                throw new InvalidOperationException($"Could not add ExpenseType '{ExpenseType}', maybe it already exists?");

            value.AddCategories(ExpenseType, _moneylist.Keys);
            field = value;
        }
    }

    internal readonly record struct ExpenseCategoryBuffer(string Label, decimal Amount);

    internal CategorizedMoneyCollection(string expenseType, List<ExpenseCategoryBuffer>? amounts = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expenseType);
        ExpenseType = expenseType;

        if (amounts is not null and { Count: > 0 }) 
            for (int i = 0; i < amounts.Count; i++)
            {
                var item = amounts[i];
                Debug.Assert(string.IsNullOrWhiteSpace(item.Label) is false);
                _moneylist.Add(item.Label, new(item.Label, item.Amount, this));
            }

        Internal_Handler_AmountChanged = new(ExpenseCategoryChanged);
    }

    public ExpenseCategory Add(string label, decimal amount)
    {
        var item = new ExpenseCategory(label, amount, this)
        {
            Internal_AmountChanged = Internal_Handler_AmountChanged
        };

        _moneylist.Add(label, new ExpenseCategory(label, amount, this));
        CategoryInfo?.AddCategory(ExpenseType, label);
        Total += item.Amount;
        RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Add, item);
        return item;
    }

    public override void Clear()
    {
        CategoryInfo?.RemoveExpenseType(ExpenseType);
        base.Clear();
    }

    public override bool Remove(ExpenseCategory item)
    {
        CategoryInfo?.RemoveCategory(ExpenseType, item.Label);
        return base.Remove(item);
    }

    public bool RenameCategory(string label, string newLabel)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        ArgumentException.ThrowIfNullOrWhiteSpace(newLabel);

        if (_moneylist.TryGetValue(label, out var item) && _moneylist.TryAdd(newLabel, item))
        {
            CategoryInfo?.RemoveCategory(ExpenseType, label);
            CategoryInfo?.AddCategory(ExpenseType, newLabel);
            _moneylist.Remove(label);
            item.Label = newLabel;
            return true;
        }

        return false;
    }

    public string ExpenseType { get; private set; }

    public bool TryRenameExpenseType(string newName)
    {
        if (CategoryInfo?.RenameExpenseType(ExpenseType, newName) is false)
        {
            ExpenseType = newName;
            return true;
        }

        return false;
    }

    internal void ClearFromRecord()
    {
        CategoryInfo?.RemoveExpenseType(ExpenseType);
        ExpenseType = null!;
    }

    IEnumerator IEnumerable.GetEnumerator() => _moneylist.GetEnumerator();

    private void ExpenseCategoryChanged(ExpenseCategory amount, decimal old, decimal @new)
    {
        var diff = @new - old;
        Total += diff;
        RaiseTotalChangedEvent(diff);
        RaiseCollectionChangedEvent(NotifyCollectionChangedAction.Replace, amount, false);
    }

    protected override void RaiseTotalChangedEvent(decimal diff)
    {
        Internal_TotalChanged?.Value?.Invoke(this, diff);
        base.RaiseTotalChangedEvent(diff);
    }

    protected override Dictionary<string, ExpenseCategory> InnerCollectionFactory(IEnumerable<ExpenseCategory>? amounts)
        => amounts is null
            ? []
            : new Dictionary<string, ExpenseCategory>(amounts.Select(x => new KeyValuePair<string, ExpenseCategory>(x.Label, x)));

    protected override ExpenseCategory GetEntry(KeyValuePair<string, ExpenseCategory> entry)
        => entry.Value;

    protected override KeyValuePair<string, ExpenseCategory> GetInnerEntry(ExpenseCategory entry)
        => new(entry.Label, entry);
}
