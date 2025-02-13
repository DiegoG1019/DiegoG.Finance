using DiegoG.Finance.Internal;
using MessagePack;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace DiegoG.Finance;

public sealed class WorkSheetBook : FinancialWork<WorkSheetBook>, IEnumerable<WorkSheetPage>
{
    private readonly Dictionary<MonthlyPeriod, WorkSheetPage> _dictionary = [];
    private readonly List<WorkSheetPage> _periods = [];

    [MessagePackObject]
    public readonly record struct Info([property: Key(0)] IEnumerable<KeyValuePair<MonthlyPeriod, WorkSheetPage.Info>> Pages);

    internal Info GetInfo()
        => new(this.Select(x => new KeyValuePair<MonthlyPeriod, WorkSheetPage.Info>(x.Period, x.GetInfo())));

    internal WorkSheetBook(WorkSheet sheet, Info? book) : base(sheet)
    {
        if (book is Info b)
        {
            foreach (var (period, page) in b.Pages)
                if (AddNoSort(period, new WorkSheetPage(period, this, page)) is false)
                    throw new ArgumentException("Encountered conflicting periods in the WorkSheetBook data", nameof(book));
            Sort();
        }
    }

    internal bool AddNoSort(MonthlyPeriod period, WorkSheetPage item)
    {
        if (_dictionary.TryAdd(period, item))
        {
            _periods.Add(item);
            return true;
        }
        return false;
    }

    internal bool Add(MonthlyPeriod period, WorkSheetPage item)
    {
        Debug.Assert(item is not null);

        if (_dictionary.TryAdd(period, item))
        {
            for (int i = _periods.Count; i > 0; i--)
                if (period > _periods[i - 1].Period)
                {
                    _periods.Insert(i, item);
                    return true;
                }

            _periods.Insert(0, item);
            return true;
        }

        return false;
    }

    internal void Sort() 
        => _periods.Sort();

    public bool AddPage(MonthlyPeriod period, [MaybeNullWhen(false)] out WorkSheetPage? value)
    {
        if (_dictionary.ContainsKey(period) is false)
        {
            value = new WorkSheetPage(period, this, null!);
            var r = Add(period, value);
            Debug.Assert(r);
            return true;
        }

        value = null;
        return false;
    }

    public bool Remove(MonthlyPeriod period)
    {
        if (_dictionary.Remove(period, out var page))
        {
            _periods.Remove(page);
            page.Invalidate();
            return true;
        }

        return false;
    }

    public bool ContainsKey(MonthlyPeriod period)
        => _dictionary.ContainsKey(period);

    public bool TryGetValue(MonthlyPeriod period, [MaybeNullWhen(false)] out WorkSheetPage? value)
        => _dictionary.TryGetValue(period, out value);

    public WorkSheetPage this[MonthlyPeriod period] => _dictionary[period];

    public WorkSheetPage this[int index] => _periods[index];

    public int Count => _periods.Count;

    public IEnumerable<MonthlyPeriod> Periods()
        => _periods.Select(x => x.Period);

    public IEnumerator<WorkSheetPage> GetEnumerator()
        => _periods.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
