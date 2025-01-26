using GLV.Shared.Blazor;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace DiegoG.Finance.Blazor.Services;

public record class ColorTheme(ImmutableArray<Color> ExpenseCategoryColors)
{
    public static ColorTheme Default { get; } = new(GLVColors.StandardColors);
}
