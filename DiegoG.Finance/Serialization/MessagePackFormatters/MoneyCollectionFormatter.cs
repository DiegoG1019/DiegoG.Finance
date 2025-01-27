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
            MessagePackSerializer.Serialize(ref writer, value._moneylist, options);
        }
    }

    public MoneyCollection? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        => reader.IsNil
            ? null
            : new MoneyCollection(MessagePackSerializer.Deserialize<HashSet<LabeledAmount>>(ref reader, options));
}