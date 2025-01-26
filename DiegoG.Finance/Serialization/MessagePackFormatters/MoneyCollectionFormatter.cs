using DiegoG.Finance.Serialization.MessagePackFormatters;
using MessagePack;
using MessagePack.Formatters;

[assembly: MessagePackKnownFormatter(typeof(MoneyCollectionFormatter))]

namespace DiegoG.Finance.Serialization.MessagePackFormatters;

public sealed class MoneyCollectionFormatter : IMessagePackFormatter<MoneyCollection?>
{
    private MoneyCollectionFormatter() { }

    public readonly static IMessagePackFormatter<MoneyCollection?> Instance = new MoneyCollectionFormatter();

    public void Serialize(ref MessagePackWriter writer, MoneyCollection? value, MessagePackSerializerOptions options)
    {
        if (value is null)
            writer.WriteNil();
        else
        {
            CurrencyFormatter.Instance.Serialize(ref writer, value.Currency, options);
            MessagePackSerializer.Serialize(ref writer, value._moneylist, options);
        }
    }

    internal static MoneyCollection? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options, IInternalMoneyCollectionParent? parent)
        => reader.IsNil
            ? null
            : parent is null 
                ? new MoneyCollection(
                    CurrencyFormatter.Instance.Deserialize(ref reader, options),
                    MessagePackSerializer.Deserialize<HashSet<LabeledAmount>>(ref reader, options)
                )
                : new MoneyCollection(
                    parent,
                    MessagePackSerializer.Deserialize<HashSet<LabeledAmount>>(ref reader, options)
                );

    public MoneyCollection? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        => Deserialize(ref reader, options, null);
}