namespace DiegoG.Finance.Blazor.Pages;

public readonly struct CollapseControl
{
    public bool IsCollapsed { get; init; }
    public string CollapseClass => IsCollapsed ? "collapsed" : "uncollapsed";
    public CollapseControl Toggle()
        => this with { IsCollapsed = !IsCollapsed };
}
