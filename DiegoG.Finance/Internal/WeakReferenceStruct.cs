using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace DiegoG.Finance.Internal;

internal readonly struct WeakReferenceStruct<T>(T target) : IDisposable
    where T : class
{
    public sealed class WeakReferenceStructEqualityComparer : IEqualityComparer<WeakReferenceStruct<T>>
    {
        private WeakReferenceStructEqualityComparer() { }

        public static WeakReferenceStructEqualityComparer Instance { get; } = new();

        public bool Equals(WeakReferenceStruct<T> x, WeakReferenceStruct<T> y) 
            => x.TryGetTarget(out var tx) == y.TryGetTarget(out var ty) && tx == ty;
        // If both are null, they are also equal.

        public int GetHashCode([DisallowNull] WeakReferenceStruct<T> obj)
            => obj.TryGetTarget(out var t) ? t.GetHashCode() : 0;
    }

    private readonly GCHandle handle = target is not null ? GCHandle.Alloc(target, GCHandleType.Weak) : throw new ArgumentNullException(nameof(target));

    public bool TryGetTarget([NotNullWhen(true)] out T target)
    {
        target = (T)handle.Target!;
        GC.KeepAlive(target);
        return true;
    }

    public bool IsAlive => handle.IsAllocated && handle.Target != null;

    public void Dispose()
    {
        if (handle.IsAllocated)
            handle.Free();
    }
}
