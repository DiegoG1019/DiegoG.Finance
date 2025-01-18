using NodaMoney;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Finance;

public class WorkSheet : IFinancialWork
{
    internal WorkSheet(int version, SpendingTrackerSheet? spendingTrackers)
    {
        Version = version;
        if (spendingTrackers is not null)
        {
            spendingTrackers.Parent = this;
            SpendingTrackers = spendingTrackers;
        }
        else
            SpendingTrackers = new SpendingTrackerSheet(this);
    }

    public WorkSheet() : this(1, null) { }

    public int Version { get; }

    public DateTimeOffset Created { get; init; } = DateTimeOffset.Now;

    public string? Title { get; init; }

    [field: AllowNull]
    public string Name
        => string.IsNullOrWhiteSpace(Title) is false ? Title : (field ??= GetPlaceholderName(Created));

    public SpendingTrackerSheet? SpendingTrackers { get; init; }

    public Currency Currency { get; set; }

    public static string GetPlaceholderName(DateTimeOffset created)
        => created.ToString("R").Replace(":", "+");
}
