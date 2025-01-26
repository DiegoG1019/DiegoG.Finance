using DiegoG.Finance.Blazor.Services;
using DiegoG.Finance.Results;
using Microsoft.AspNetCore.Components;

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
        WorkTable.CurrentSheetMemberChanged += WorkTable_CurrentSheetMemberChanged;
    }

    private void WorkTable_CurrentSheetMemberChanged()
    {
        StateHasChanged();
        log?.LogDebug("Reloaded CurrentSheet");
    }

    private void Language_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        StateHasChanged();
        log?.LogDebug("Reloaded CurrentSheet due to LanguageChange");
    }

    private void Result_GoalChanged(SpendingTrackerCategoryResult result, Percentage old, Percentage @new)
    {
        StateHasChanged();
        log?.LogDebug("Reloaded CurrentSheet due to Result Goal Change");
    }
}
