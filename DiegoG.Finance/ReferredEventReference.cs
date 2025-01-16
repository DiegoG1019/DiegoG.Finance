namespace DiegoG.Finance;

internal class ReferredEventReference<TFunc>(TFunc? @event) where TFunc : Delegate
{
    public TFunc? Event { get; set; } = @event;
}
