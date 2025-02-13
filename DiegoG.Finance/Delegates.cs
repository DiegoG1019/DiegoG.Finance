using DiegoG.Finance.Internal;
using Microsoft.VisualBasic;
using NodaMoney;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Finance;

public delegate void FinancialCollectionChangedEventHandler<TSender, TItem>(TSender sender, NotifyCollectionChangedAction Action, TItem? item);

public delegate void FinancialWorkValueChangedHandler<TSender, TValue>(TSender sender, TValue? oldValue, TValue newValue)
    where TSender : FinancialWork<TSender>;

public delegate void FinancialWorkInvalidatedHandler<TParent, TSender>(TSender sender)
    where TParent : FinancialWork<TParent>
    where TSender : FinancialEntity<TParent, TSender>;