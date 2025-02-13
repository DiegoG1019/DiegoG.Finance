using NodaMoney;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Finance;

public sealed class WorkSheet
{
    public delegate void WorkSheetPropertyChangedEventHandler(WorkSheet sender, string property);

    internal WorkSheet(int version)
    {
        Version = version;
    }

    public WorkSheet() : this(1)
    {
        ExpenseTypesAndCategories = new(this, null);
        Book = new(this, null);
    }

    public WorkSheet(
        Currency currency,
        IEnumerable<ExpenseType.Info>? typesAndCategories = null,
        WorkSheetBook.Info? book = null
    ) : this(1)
    {
        ExpenseTypesAndCategories = new(this, typesAndCategories);
        Book = new(this, book);
    }


    [field: AllowNull]
    public ExpenseTypesCollection ExpenseTypesAndCategories
    {
        get
        {
            Debug.Assert(field is not null);
            return field;
        }
    }

    [field: AllowNull]
    public WorkSheetBook Book
    {
        get
        {
            Debug.Assert(field is not null);
            return field;
        }
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
