using System;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Reflection;

namespace DiegoG.Finance.Blazor.Services;

public sealed class Language
{
    public required string LanguageName { get; set; }
    public required string LanguageCode { get; init; }
    public required string CVUri { get; init; }
    public required string Home { get; init; }
    public required string SiteTitle { get; init; }
    public required string ShortSiteTitle { get; init; }
    public required string SubmitButton { get; init; }
    public required string NotFoundTitle { get; init; }
    public required string NotFoundMessage { get; init; }
    public required string ContactMe { get; init; }
}

public static class AvailableLanguages
{
    static AvailableLanguages()
    {
        Español = new Language()
        {
            LanguageName = "Español",
            LanguageCode = "esp",
            CVUri = "/Diego CV - ESPAÑOL - DEC24.pdf",
            Home = "Principal",
            SiteTitle = "Tracker de Finanzas | Dev DiegoG",
            ShortSiteTitle = "Finanzas | DiegoG",
            SubmitButton = "Subir",
            NotFoundTitle = "No Encontrado",
            NotFoundMessage = "Lo siento, no hay nada en esta dirección",
            ContactMe = "Contáctame!",
        };

        English = new Language()
        {
            LanguageName = "English",
            LanguageCode = "eng",
            CVUri = "/Diego CV - ENGLISH - DEC24.pdf",
            Home = "Home",
            SiteTitle = "Finance Tracker | Dev DiegoG",
            ShortSiteTitle = "Finances | DiegoG",
            SubmitButton = "Submit",
            NotFoundTitle = "Not Found",
            NotFoundMessage = "Sorry, there's nothing at this address",
            ContactMe = "Contact Me!",
        };

        Languages = typeof(AvailableLanguages).GetProperties()
                                                  .Where(x => x.PropertyType == typeof(Language))
                                                  .Select(x => x.GetValue(null))
                                                  .Cast<Language>()
                                                  .ToFrozenDictionary(
                                                      v => v.LanguageCode,
                                                      k => k,
                                                      StringComparer.OrdinalIgnoreCase
                                                  );
    }

    public static FrozenDictionary<string, Language> Languages { get; }

    public static Language Español { get; }

    public static Language English { get; }
}
