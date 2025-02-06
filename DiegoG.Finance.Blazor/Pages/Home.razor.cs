using DiegoG.Finance.Blazor.ModelControls;
using DiegoG.Finance.Blazor.Services;
using Microsoft.AspNetCore.Components;
using NodaMoney;

namespace DiegoG.Finance.Blazor.Pages;

public partial class Home
{
    [CascadingParameter]
    public ContextProvider Context { get; set; }

    [CascadingParameter]
    public WorkTable WorkTable { get; set; }

    private readonly List<WorkSheetHeader> Headers = [];

    protected override async Task OnInitializedAsync()
    {
        await foreach (var header in storage.EnumerateAvailableWorksheets())
        {
            Headers.Add(header);
            StateHasChanged();
        }

        Context.PropertyChanged += Language_PropertyChanged;
    }

    private void Language_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        StateHasChanged();
    }

    private Task GoToWorkPage()
        => GoToWorkPage(null);

    private async Task GoToWorkPage(WorkSheetHeader? header)
    {
        WorkTable.CurrentSheet = header is WorkSheetHeader h ? await storage.LoadWorkSheet(h) : WorkTable.NewDefaultWorkSheet();
        nav.NavigateTo("/work");
    }
}
