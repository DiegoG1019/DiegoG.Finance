using DiegoG.Finance.Serialization.MessagePackFormatters;
using MessagePack;
using MessagePack.Formatters;
using Newtonsoft.Json.Linq;

[assembly: MessagePackKnownFormatter(typeof(SpendingTrackerSheetFormatter))]

namespace DiegoG.Finance.Serialization.MessagePackFormatters;

public sealed class SpendingTrackerSheetFormatter : IMessagePackFormatter<SpendingTrackerSheet?>
{
    private SpendingTrackerSheetFormatter() { }

    public readonly static IMessagePackFormatter<SpendingTrackerSheet?> Instance = new SpendingTrackerSheetFormatter();
    
    public void Serialize(ref MessagePackWriter writer, SpendingTrackerSheet? value, MessagePackSerializerOptions options)
    {
        if (value is null)
            writer.WriteNil();
        else
        {
            CategorizedMoneyCollectionFormatter.Instance.Serialize(ref writer, value.IncomeSources, options);
            ExpenseTypesCollectionFormatter.Instance.Serialize(ref writer, value.ExpenseCategories, options);
        }
    }

    public SpendingTrackerSheet? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) 
        => reader.IsNil
            ? null
            : new SpendingTrackerSheet(
                CategorizedMoneyCollectionFormatter.Instance.Deserialize(ref reader, options),
                ExpenseTypesCollectionFormatter.Instance.Deserialize(ref reader, options)
            );
}
