using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace DiegoG.Finance;

public sealed class FinancialCategoryCollection : IReadOnlyDictionary<string, IReadOnlySet<string>>
{
    public delegate void FinancialCategoryCollectionChangedEventHandler(FinancialCategoryCollection sender, NotifyCollectionChangedAction Action, string expenseType, string category);

    private readonly Dictionary<string, HashSet<string>> _list = [];

    public bool ContainsKey(string key)
    {
        return ((IReadOnlyDictionary<string, IReadOnlySet<string>>)_list).ContainsKey(key);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out IReadOnlySet<string> value)
    {
        return ((IReadOnlyDictionary<string, IReadOnlySet<string>>)_list).TryGetValue(key, out value);
    }

    public IReadOnlySet<string> this[string key] => ((IReadOnlyDictionary<string, IReadOnlySet<string>>)_list)[key];

    public IEnumerable<string> Keys => ((IReadOnlyDictionary<string, IReadOnlySet<string>>)_list).Keys;

    public IEnumerable<IReadOnlySet<string>> Values => ((IReadOnlyDictionary<string, IReadOnlySet<string>>)_list).Values;

    public int Count => ((IReadOnlyCollection<KeyValuePair<string, IReadOnlySet<string>>>)_list).Count;

    public IEnumerator<KeyValuePair<string, IReadOnlySet<string>>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, IReadOnlySet<string>>>)_list).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_list).GetEnumerator();
    }

    public event FinancialCollectionChangedEventHandler<FinancialCategoryCollection, string>? ExpenseTypeChanged;
    public event FinancialCategoryCollectionChangedEventHandler? ExpenseCategoryChanged;

    internal bool AddExpenseType(string expenseType)
    {
        if (_list.ContainsKey(expenseType) is false && _list.TryAdd(expenseType, []))
        {
            ExpenseTypeChanged?.Invoke(this, NotifyCollectionChangedAction.Add, expenseType);
            return true;
        }

        return false;
    }

    internal bool RemoveExpenseType(string expenseType)
    {
        if (_list.Remove(expenseType))
        {
            ExpenseTypeChanged?.Invoke(this, NotifyCollectionChangedAction.Remove, expenseType);
            return true;
        }

        return false;
    }

    internal bool RenameExpenseType(string expenseType, string newName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expenseType);
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);

        if (_list.TryGetValue(expenseType, out var value) && _list.TryAdd(newName, value)) 
        {
            _list.Remove(expenseType);
            ExpenseTypeChanged?.Invoke(this, NotifyCollectionChangedAction.Move, expenseType);
            return true;
        }

        return false;
    }

    internal bool RemoveCategory(string expenseType, string category)
    {
        if (_list.TryGetValue(expenseType, out var type) is false) { }
            Debug.Fail($"expenseType '{expenseType}' could not be found while attempting to remove category '{category}'");

        if (type.Remove(category))
        {
            ExpenseCategoryChanged?.Invoke(this, NotifyCollectionChangedAction.Remove, expenseType, category);
            return true;
        }

        return false;
    }

    internal int AddCategories(string expenseType, params IEnumerable<string> categories)
    {
        if (_list.TryGetValue(expenseType, out var type) is false)
            Debug.Fail($"expenseType '{expenseType}' could not be found while attempting to add categories");

        int added = 0;
        foreach(var category in categories)
        {
            type.Add(category);
            added++;
        }

        if (added > 0)
            ExpenseCategoryChanged?.Invoke(this, NotifyCollectionChangedAction.Reset, expenseType, null!);

        return added;
    }

    internal int AddCategories(string expenseType, params Span<string> categories)
    {
        if (_list.TryGetValue(expenseType, out var type) is false)
            Debug.Fail($"expenseType '{expenseType}' could not be found while attempting to add categories");

        int added = 0;
        for (int i = 0; i < categories.Length; i++)
        {
            type.Add(categories[i]);
            added++;
        }
        
        if (added > 0)
            ExpenseCategoryChanged?.Invoke(this, NotifyCollectionChangedAction.Reset, expenseType, null!);

        return added;
    }

    internal bool AddCategory(string expenseType, string category)
    {
        if (_list.TryGetValue(expenseType, out var type) is false)
            Debug.Fail($"expenseType '{expenseType}' could not be found while attempting to add category '{category}'");

        if (type.Add(category))
        {
            ExpenseCategoryChanged?.Invoke(this, NotifyCollectionChangedAction.Add, expenseType, category);
            return true;
        }

        return false;
    }
}
