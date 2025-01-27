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
    {
        //WorkTable.CurrentSheet = new();
        nav.NavigateTo("/work");
        return Task.CompletedTask;
    }
}
