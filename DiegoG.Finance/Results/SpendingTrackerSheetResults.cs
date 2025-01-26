using NodaMoney;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using static DiegoG.Finance.SpendingTrackerSheet;

namespace DiegoG.Finance.Results;

public sealed class SpendingTrackerSheetResults : IReadOnlyDictionary<string, SpendingTrackerCategoryResult>
{
    internal readonly Dictionary<string, SpendingTrackerCategoryResult> results = [];
    private readonly SpendingTrackerSheet Sheet;

    internal SpendingTrackerSheetResults(SpendingTrackerSheet sheet)
    {
        Sheet = sheet ?? throw new ArgumentNullException(nameof(sheet));
    }

    public bool ContainsKey(string key)
    {
        return results.ContainsKey(key);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out SpendingTrackerCategoryResult value)
    {
        return results.TryGetValue(key, out value);
    }

    public SpendingTrackerCategoryResult this[string key] => results[key];

    public IEnumerable<string> Keys => results.Keys;

    public IEnumerable<SpendingTrackerCategoryResult> Values => results.Values;

    public int Count => results.Count;

    public IEnumerator<KeyValuePair<string, SpendingTrackerCategoryResult>> GetEnumerator()
        => results.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => results.GetEnumerator();

    internal SpendingTrackerCategoryResult Add(
        string key, 
        Percentage goal, 
        MoneyCollection collection, 
        CategorizedMoneyCollection categorizedMoneyCollection
    )
    {
        var result = new SpendingTrackerCategoryResult(goal, collection, categorizedMoneyCollection, Sheet);
        results.Add(key, result);
        return result;
    }

    internal bool Remove(string key)
        => results.Remove(key);

    internal void Clear()
        => results.Clear();

    public Percentage RemainingPercentage => Percentage.FromRatio(Remaining, Sheet.IncomeSources.Total);

    [JsonIgnore]
    public decimal Remaining => Sheet.IncomeSources.Total - Sheet.ExpenseCategories.Total;

    [JsonIgnore]
    public Money RemainingMoney => new(Remaining, Sheet.Currency);
}
