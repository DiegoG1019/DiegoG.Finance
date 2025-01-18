using DiegoG.Finance.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace DiegoG.Finance.Blazor.Pages;

public partial class WorkTablePage
{
    [CascadingParameter]
    public LanguageProvider Language { get; set; }

    [CascadingParameter]
    public WorkTable WorkTable { get; set; }

    private readonly CollapseControl[] CollapseControls = new CollapseControl[1];
}
