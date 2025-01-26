using NodaMoney;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DiegoG.Finance.WorkSheet;

namespace DiegoG.Finance;

public class WorkSheet : IFinancialWork, IInternalSpendingTrackerSheetParent
{
    public delegate void WorkSheetPropertyChangedEventHandler(WorkSheet sender, string property);

    internal WorkSheet(int version, SpendingTrackerSheet? spendingTrackers)
    {
        Version = version;
        internal_SpendingTrackerSheetMemberChanged = new(handler_internal_WorkSheetSpendingTrackerSheetMemberChanged);
        if (spendingTrackers is not null)
        {
            spendingTrackers.Parent = this;
            SpendingTrackers = spendingTrackers;
        }
        else
            SpendingTrackers = new SpendingTrackerSheet(null, null, null);

        SpendingTrackers.Parent = this;
    }

    public WorkSheet(Currency? currency = null) : this(1, null)
    {
        Currency = currency ?? Currency.CurrentCurrency;
    }

    public int Version { get; }

    public DateTimeOffset Created { get; init; } = DateTimeOffset.Now;

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
    public event Action<WorkSheet>? WorkSheetSpendingTrackerSheetMemberChanged;

    public static string GetPlaceholderName(DateTimeOffset created)
        => $"DiegoG.Finance Worksheet {created.ToString("s", CultureInfo.CurrentCulture).Replace(':', '.')}";

    private readonly ReferredReference<Action<SpendingTrackerSheet>> internal_SpendingTrackerSheetMemberChanged;

    private void handler_internal_WorkSheetSpendingTrackerSheetMemberChanged(SpendingTrackerSheet sheet)
        => WorkSheetSpendingTrackerSheetMemberChanged?.Invoke(this);

    ReferredReference<Action<SpendingTrackerSheet>> IInternalSpendingTrackerSheetParent.Internal_MemberChanged
        => internal_SpendingTrackerSheetMemberChanged;
}
