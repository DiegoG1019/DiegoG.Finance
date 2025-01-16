using NodaMoney;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Text.Json.Serialization;

namespace DiegoG.Finance;

public class SpendingTrackerSheet : ISheet
{
    private readonly Dictionary<string, SpendingTrackerCategoryResult> results = [];

    public class SpendingTrackerCategoryResult(Percentage goal, MoneyCollection collection, SpendingTrackerSheet sheet)
    {
        public MoneyCollection Collection { get; } = collection ?? throw new ArgumentNullException(nameof(collection));
        public SpendingTrackerSheet Sheet { get; } = sheet ?? throw new ArgumentNullException(nameof(sheet));
        public Percentage Goal { get; set; } = goal;
        public Percentage ActualPercentage => Percentage.FromRatio(sheet.IncomeSources.Total, Collection.Total);
        public Money Total => Collection.MoneyTotal;
    }

    public SpendingTrackerSheet(
        Currency currency,
        MoneyCollection? incomeSources = null,
        CategorizedMoneyCollection? expenseCategories = null
    )
    {
        Currency = currency;
        
        if (incomeSources is null)
            IncomeSources = new(Currency);
        else
        {
            if (incomeSources.Currency != Currency)
                throw new ArgumentException($"A SpendingTrackerSheet of Currency {Currency} cannot have an IncomeSources collection of Currency {incomeSources.Currency}");
            IncomeSources = incomeSources;
        }

        if (expenseCategories is null)
            ExpenseCategories = new(Currency);
        else
        {
            if (expenseCategories.Currency != Currency)
                throw new ArgumentException($"A SpendingTrackerSheet of Currency {Currency} cannot have an ExpenseCategories collection of Currency {expenseCategories.Currency}");
            ExpenseCategories = expenseCategories;
        }

        ExpenseCategories.CollectionChanged += ExpenseCategories_CollectionChanged;
    }

    private void ExpenseCategories_CollectionChanged(
        CategorizedFinancialCollection<MoneyCollection> arg1, 
        NotifyCollectionChangedAction arg2, 
        KeyValuePair<string, MoneyCollection>? arg3
    )
    {
        if (arg2 is NotifyCollectionChangedAction.Add)
        {
            Debug.Assert(arg3.HasValue, "ExpenseCategories collection changed with an addition; but arg3 was null");
            results.Add(arg3.Value.Key, new SpendingTrackerCategoryResult(new(0), arg3.Value.Value, this));
        }    
        else if (arg2 is NotifyCollectionChangedAction.Remove)
        {
            Debug.Assert(arg3.HasValue, "ExpenseCategories collection changed with a removal; but arg3 was null");
            results.Remove(arg3.Value.Key);
        }
        else if (arg2 is NotifyCollectionChangedAction.Reset)
        {
            results.Clear();
            foreach (var item in arg1)
                results.Add(item.Key, new SpendingTrackerCategoryResult(new(0), item.Value, this));
        }
    }

    public Currency Currency { get; }
    public MoneyCollection IncomeSources { get; } 
    public CategorizedMoneyCollection ExpenseCategories { get; }

    [JsonIgnore]
    public IReadOnlyDictionary<string, SpendingTrackerCategoryResult> Results => results;

    [JsonIgnore]
    public decimal Remaining => IncomeSources.Total - ExpenseCategories.Total;

    [JsonIgnore]
    public Money RemainingMoney => new(Remaining, Currency);
}
