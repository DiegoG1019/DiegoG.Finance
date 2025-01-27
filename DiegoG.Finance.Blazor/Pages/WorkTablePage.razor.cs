using DiegoG.Finance.Blazor.ModelControls;
using DiegoG.Finance.Blazor.Services;
using DiegoG.Finance.Results;
using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;

namespace DiegoG.Finance.Blazor.Pages;

public partial class WorkTablePage(ILogger<WorkTablePage> log)
{
    private readonly CollapseControl[] CollapseControls = new CollapseControl[1];

    [CascadingParameter]
    public ContextProvider Context { get; set; }

    [CascadingParameter]
    public WorkTable WorkTable { get; set; }

    protected override void OnInitialized()
    {
        Context.PropertyChanged += Language_PropertyChanged;
        WorkTable.PreAnalyzeSheet();
        WorkTable.CurrentSheetChanged += WorkTable_CurrentSheetChanged;
    }

    #region Event Handlers

    private void WorkTable_CurrentSheetChanged()
    {
        StateHasChanged();
        log?.LogDebug("Reloaded CurrentSheet");
    }

    private void Language_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        StateHasChanged();
        log?.LogDebug("Reloaded CurrentSheet due to LanguageChange");
    }

    private void FinancialCollectionEventHandler<TSender, TItem>(TSender sender, NotifyCollectionChangedAction Action, TItem? item)
    {
        StateHasChanged();
    }

    private void FinancialWorkEventHandler<TSender, TValue>(TSender result, TValue old, TValue @new)
    {
        StateHasChanged();
    }

    #endregion
}
