using DiegoG.Finance.Internal;
using MessagePack;
using System.Collections;
using System.Runtime.InteropServices;

namespace DiegoG.Finance;

public sealed class MoneyMovementTracker : PagedEntity<WorkSheetPage, MoneyMovementTracker>, IReadOnlyCollection<MoneyMovementEntry>
{
    private readonly List<MoneyMovementEntry> _list = [];

    [MessagePackObject]
    public readonly record struct Info([property: Key(0)] IEnumerable<MoneyMovementEntry.Info> Entries);

    public Info GetInfo()
        => new(_list.Select(x => x.GetInfo()));

    internal MoneyMovementTracker(WorkSheetPage parent, Info? info) : base(parent)
    {
        if (info is Info i)
            foreach (var entryInfo in i.Entries)
            {
                var entry = new MoneyMovementEntry(this, entryInfo);
                _list.Add(entry);
                entry.Internal_DateChanged += Entry_Internal_DateChanged;
            }

        _list.Sort(MoneyMovementEntry.Comparer.Instance);
    }

    public int Count => _list.Count;

    public IEnumerator<MoneyMovementEntry> GetEnumerator()
        => _list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _list.GetEnumerator();

    public decimal Total { get; }

    public decimal TotalSpent { get; }

    public MoneyMovementEntry Add(DateTime date, ExpenseCategory category)
    {
        var entry = new MoneyMovementEntry(this, category, date);
        entry.Internal_DateChanged += Entry_Internal_DateChanged;

        lock (_list)
        {
            var indx = _list.BinarySearch(entry);
            if (indx > 0)
                _list.Insert(indx, entry);
            else
                _list.Insert(~indx, entry);
        }

        return entry;
    }

    private void Entry_Internal_DateChanged(MoneyMovementEntry sender, DateTime oldValue, DateTime newValue)
    {
        lock (_list)
        {
            _list.Remove(sender);
            var indx = _list.BinarySearch(sender);
            if (indx > 0)
                _list.Insert(indx, sender);
            else
                _list.Insert(~indx, sender);
        }
    }

    public MoneyMovementTotalTracker GetTotalInCategory(ExpenseCategory category)
        => Page.GetOrAddCategoryTracker<MoneyMovementTotalTracker>(category, MoneyMovementTotalTracker.TrackerFactory);

    public MoneyMovementTotalTracker GetTotalInType(ExpenseType expType)
        => Page.GetOrAddTypeTracker<MoneyMovementTotalTracker>(expType, MoneyMovementTotalTracker.TrackerFactory);
}
