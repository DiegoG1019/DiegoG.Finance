using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DiegoG.Finance.Blazor.Services;

public sealed class ContextProvider : INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs langChangedArgs = new(nameof(CurrentLanguage));
    private static readonly PropertyChangedEventArgs themeChangedArgs = new(nameof(CurrentTheme));

    public ContextProvider()
    {
        var langcode = CultureInfo.CurrentCulture.ThreeLetterISOLanguageName;
        if (AvailableLanguages.Languages.TryGetValue(langcode, out var lang))
            _currentLanguage = lang;
        else
        {
            Console.WriteLine($"Could not find a language match for code {langcode}; falling back to English");
            _currentLanguage = AvailableLanguages.English;
        }
    }

    private Language _currentLanguage;
    public Language CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == _currentLanguage) return;
            _currentLanguage = value;
            PropertyChanged?.Invoke(this, langChangedArgs);
        }
    }

    [field: AllowNull]
    public ColorTheme CurrentTheme
    {
        get => field;
        set
        {
            if (value != field)
            {
                field = value ?? ColorTheme.Default;
                PropertyChanged?.Invoke(this, themeChangedArgs);
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
