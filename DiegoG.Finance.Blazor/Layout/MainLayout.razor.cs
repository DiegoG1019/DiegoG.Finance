using DiegoG.Finance.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DiegoG.Finance.Blazor.Layout;

public partial class MainLayout
{
    [CascadingParameter]
    public LanguageProvider Language { get; set; }

    private string clang;
    public string CurrentLanguage
    {
        get => clang;
        set
        {
            Language.CurrentLanguage = AvailableLanguages.Languages[clang = value];
            StateHasChanged();
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        clang = Language.CurrentLanguage.LanguageCode;
    }
}
