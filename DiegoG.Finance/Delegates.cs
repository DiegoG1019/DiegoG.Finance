using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiegoG.Finance;

public delegate void FinancialCollectionChangedEventHandler<TSender, TItem>(TSender sender, NotifyCollectionChangedAction Action, TItem? item);

public delegate void FinancialWorkEventHandler<TSender, TValue>(TSender sender, TValue oldValue, TValue newValue);
