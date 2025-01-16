using MessagePack;
using MessagePack.Formatters;
using NodaMoney;

namespace DiegoG.Finance.Serialization.MessagePackFormatters;

public sealed class FinanceResolver : IFormatterResolver
{
    private FinanceResolver() { }

    public static readonly IFormatterResolver Instance = new FinanceResolver();

    public IMessagePackFormatter<T>? GetFormatter<T>()
    {
        if (typeof(T) == typeof(Currency))
            return (IMessagePackFormatter<T>)CurrencyFormatter.Instance;

        else if (typeof(T) == typeof(MoneyCollection))
            return (IMessagePackFormatter<T>)MoneyCollectionFormatter.Instance;

        else if (typeof(T) == typeof(CategorizedMoneyCollection))
            return (IMessagePackFormatter<T>)CategorizedMoneyCollectionFormatter.Instance;

        else if (typeof(T) == typeof(LabeledAmount))
            return (IMessagePackFormatter<T>)LabeledAmountFormatter.Instance;

        else if (typeof(T) == typeof(SpendingTrackerSheet))
            return (IMessagePackFormatter<T>)SpendingTrackerSheetFormatter.Instance;

        return null;
    }
}
