namespace DiegoG.Finance;

public sealed class ReferredReference<T>(T value) 
{
    public T Value { get; set; } = value;
}
