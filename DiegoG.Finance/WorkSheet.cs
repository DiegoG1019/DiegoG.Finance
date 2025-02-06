using NodaMoney;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Finance;

public class MoneyFlowTracker
{
    // TODO Add functionality to stream data from a source; so that we can page the flow instead of loading it all at once. Next file version?
    
}

public class WorkSheet
{
    public delegate void WorkSheetPropertyChangedEventHandler(WorkSheet sender, string property);

    internal WorkSheet(int version, SpendingTrackerSheet? spendingTrackers)
    {
        Version = version;
        SpendingTrackers = spendingTrackers is not null ? spendingTrackers : new SpendingTrackerSheet();
        Categories = new CategorizedMoneyCollectionCategoriesSetWrapper(SpendingTrackers.ExpenseCategories);
    }

    public WorkSheet(Currency? currency = null) : this(1, null)
    {
        Currency = currency ?? Currency.CurrentCurrency;
    }

    public int Version { get; }

    public DateTimeOffset Created { get; init; } = DateTimeOffset.Now;

    public ICollection<string> Categories { get; }

    [field: AllowNull]
    public string Title
    {
        get => field ??= GetPlaceholderName(Created);
        set
        {
            field = value ?? GetPlaceholderName(Created);
            WorkSheetPropertyChanged?.Invoke(this, nameof(Title));
        }
    }

    public SpendingTrackerSheet SpendingTrackers { get; }

    public Currency Currency
    {
        get => field;
        set
        {
            field = value;
            WorkSheetPropertyChanged?.Invoke(this, nameof(Currency));
        }
    }

    public event WorkSheetPropertyChangedEventHandler? WorkSheetPropertyChanged;

    public static string GetPlaceholderName(DateTimeOffset created)
        => $"DiegoG.Finance Worksheet {created.ToString("s", CultureInfo.CurrentCulture).Replace(':', '.')}";
}
