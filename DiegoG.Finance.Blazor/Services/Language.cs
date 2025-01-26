using System;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;

namespace DiegoG.Finance.Blazor.Services;

public sealed class Language
{
    public required CultureInfo Culture { get; init; }
    public required string LanguageName { get; init; }
    public required string LanguageCode { get; init; }
    public required string CVUri { get; init; }
    public required string Home { get; init; }
    public required string SiteTitle { get; init; }
    public required string ShortSiteTitle { get; init; }
    public required string SubmitButton { get; init; }
    public required string NotFoundTitle { get; init; }
    public required string NotFoundMessage { get; init; }
    public required string ContactMe { get; init; }
    public required string CreatedDate { get; init; }
    public required string SourceCode { get; init; }

    public required string SelectWorkSheet { get; init; }
    public required string UploadWorkSheet { get; init; }
    public required string CreateNewWorkSheet { get; init; }
    public required string ProtectWorkSheet { get; init; }
    public required string Password { get; init; }
    public required string OpenWorkSheet { get; init; }
    public required string WorkSheetVersion { get; init; }
}

public static class AvailableLanguages
{
    static AvailableLanguages()
    {
        Español = new Language()
        {
            Culture = CultureInfo.GetCultureInfo("es"),

            LanguageName = "Español",
            LanguageCode = "esp",
            CVUri = "/Diego CV - ESPAÑOL - DEC24.pdf",
            Home = "Principal",
            SiteTitle = "Dev DiegoG | Tracker de Finanzas",
            ShortSiteTitle = "Dev DiegoG | Finanzas",
            SubmitButton = "Subir",
            NotFoundTitle = "No Encontrado",
            NotFoundMessage = "Lo siento, no hay nada en esta dirección",
            ContactMe = "Contáctame!",
            CreatedDate = "Fecha de Creación",
            SourceCode = "Ver Fuente",

            SelectWorkSheet = "Selecciona una hoja de trabajo",
            UploadWorkSheet = "Sube una hoja de trabajo",
            CreateNewWorkSheet = "Crear una nueva hoja de trabajo",
            ProtectWorkSheet = "Proteger hoja de trabajo",
            Password = "Contraseña",
            OpenWorkSheet = "Abrir hoja de trabajo",
            WorkSheetVersion = "Versión de hoja"
        };

        English = new Language()
        {
            Culture = CultureInfo.GetCultureInfo("en"),

            LanguageName = "English",
            LanguageCode = "eng",
            CVUri = "/Diego CV - ENGLISH - DEC24.pdf",
            Home = "Home",
            SiteTitle = "Dev DiegoG | Finance Tracker",
            ShortSiteTitle = "Dev DiegoG | Finances",
            SubmitButton = "Submit",
            NotFoundTitle = "Not Found",
            NotFoundMessage = "Sorry, there's nothing at this address",
            ContactMe = "Contact Me!",
            CreatedDate = "Creation Date",
            SourceCode = "View Source",

            SelectWorkSheet = "Select a worksheet",
            UploadWorkSheet = "Upload a worksheet",
            CreateNewWorkSheet = "Create a new worksheet",
            ProtectWorkSheet = "Protect worksheet info",
            Password = "Input a password for the worksheet",
            OpenWorkSheet = "Open worksheet",
            WorkSheetVersion = "Worksheet version"
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
