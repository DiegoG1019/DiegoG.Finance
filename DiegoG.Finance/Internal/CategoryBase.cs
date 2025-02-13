using System.Collections.Concurrent;
using System.Diagnostics;

namespace DiegoG.Finance.Internal;

public class CategoryBase<TParent, TSelf> : FinancialEntity<TParent, TSelf>
    where TParent : FinancialWork<TParent>
    where TSelf : CategoryBase<TParent, TSelf>
{
    internal CategoryBase(TParent parent) : base(parent) { }
}
