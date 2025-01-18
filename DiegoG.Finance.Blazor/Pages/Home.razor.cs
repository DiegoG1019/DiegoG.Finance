using DiegoG.Finance.Blazor.Services;
using Microsoft.AspNetCore.Components;
using NodaMoney;

namespace DiegoG.Finance.Blazor.Pages;

public partial class Home
{
    [CascadingParameter]
    public LanguageProvider Language { get; set; }

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
    }

    private Task GoToWorkPage()
    {
        WorkTable.CurrentSheet = new(new(Currency.CurrentCurrency));
        nav.NavigateTo("/work");
        return Task.CompletedTask;
    }
}
