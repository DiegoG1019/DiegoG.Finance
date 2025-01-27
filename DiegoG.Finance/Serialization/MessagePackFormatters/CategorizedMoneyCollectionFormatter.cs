using DiegoG.Finance.Serialization.MessagePackFormatters;
using MessagePack;
using MessagePack.Formatters;
using System.Text.Json;

[assembly: MessagePackKnownFormatter(typeof(CategorizedMoneyCollectionFormatter))]

namespace DiegoG.Finance.Serialization.MessagePackFormatters;

public sealed class CategorizedMoneyCollectionFormatter : IMessagePackFormatter<CategorizedMoneyCollection?>
{
    private CategorizedMoneyCollectionFormatter() { }

    public readonly static IMessagePackFormatter<CategorizedMoneyCollection?> Instance = new CategorizedMoneyCollectionFormatter();

    public void Serialize(ref MessagePackWriter writer, CategorizedMoneyCollection? value, MessagePackSerializerOptions options)
    {
        if (value is null)
            writer.WriteNil();
        else
        {
            MessagePackSerializer.Serialize(ref writer, value._categories, options);
        }
    }

    public CategorizedMoneyCollection? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) 
        => reader.IsNil
            ? null
            : new CategorizedMoneyCollection(
                MessagePackSerializer.Deserialize<Dictionary<string, MoneyCollection>>(ref reader, options)
            );
}