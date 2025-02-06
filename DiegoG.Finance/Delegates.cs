using NodaMoney;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Finance;

public delegate void FinancialCollectionChangedEventHandler<TSender, TItem>(TSender sender, NotifyCollectionChangedAction Action, TItem? item);

public delegate void FinancialWorkEventHandler<TSender, TValue>(TSender sender, TValue oldValue, TValue newValue);

public delegate void MoneyCollectionTotalChangedEventHandler<TCollection, TEntry>(TCollection sender, decimal difference)
    where TCollection : MoneyCollectionBase<TCollection, TEntry>
    where TEntry : IFinancialEntry;

public delegate void MoneyCollectionTotalChangedEventHandler<TCollection, TEntry, TInnerCollection, TInnerEntry>(TCollection sender, decimal difference)
    where TCollection : MoneyCollectionBase<TCollection, TEntry, TInnerCollection, TInnerEntry>
    where TEntry : IFinancialEntry
    where TInnerCollection : ICollection<TInnerEntry>;

public delegate void MoneyCollectionCurrencyChangedEventHandler<TCollection, TEntry>(TCollection sender, Currency newCurrency)
    where TCollection : MoneyCollectionBase<TCollection, TEntry>
    where TEntry : IFinancialEntry;
