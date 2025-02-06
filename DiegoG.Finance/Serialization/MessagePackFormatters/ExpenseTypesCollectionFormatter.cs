using DiegoG.Finance.Serialization.MessagePackFormatters;
using MessagePack;
using MessagePack.Formatters;
using System.Linq;
using System.Text.Json;

[assembly: MessagePackKnownFormatter(typeof(ExpenseTypesCollectionFormatter))]

namespace DiegoG.Finance.Serialization.MessagePackFormatters;

public sealed class ExpenseTypesCollectionFormatter : IMessagePackFormatter<ExpenseTypesCollection?>
{
    private ExpenseTypesCollectionFormatter() { }

    public readonly static IMessagePackFormatter<ExpenseTypesCollection?> Instance = new ExpenseTypesCollectionFormatter();

    public void Serialize(ref MessagePackWriter writer, ExpenseTypesCollection? value, MessagePackSerializerOptions options)
    {
        if (value is null)
            writer.WriteNil();
        else
        {
            MessagePackSerializer.Serialize(ref writer, value._categories, options);
        }
    }

    public ExpenseTypesCollection? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) 
        => reader.IsNil
            ? null
            : new ExpenseTypesCollection(
                MessagePackSerializer.Deserialize<Dictionary<string, CategorizedMoneyCollection>>(ref reader, options)
            );
}