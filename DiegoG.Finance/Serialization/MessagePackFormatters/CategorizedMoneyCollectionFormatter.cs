using DiegoG.Finance.Serialization.MessagePackFormatters;
using MessagePack;
using MessagePack.Formatters;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
            writer.WriteArrayHeader(value._moneylist.Count);
            foreach (var item in value._moneylist.Values)
            {
                if (item == null)
                    writer.WriteNil();
                else
                {
                    decimal val = item.Amount;
                    var values = MemoryMarshal.Cast<decimal, long>(MemoryMarshal.CreateSpan(ref val, 1));
                    Debug.Assert(values.Length == 2);

                    writer.Write(values[0]);
                    writer.Write(values[1]);
                    writer.Write(item.Label);
                }
            }
        }
    }

    public CategorizedMoneyCollection? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.IsNil)
            return null;
        else
        {
            int count = reader.ReadArrayHeader();
            List<CategorizedMoneyCollection.ExpenseCategoryBuffer> categories = new(count);
            Span<long> values = stackalloc long[2];
            for (; count >= 0; count--)
            {
                if (reader.IsNil) return null;

                values[0] = reader.ReadInt64();
                values[1] = reader.ReadInt64();

                var label = reader.ReadString();
                categories.Add(new CategorizedMoneyCollection.ExpenseCategoryBuffer(label ?? "?", MemoryMarshal.Cast<long, decimal>(values)[0]));
            }

            return new CategorizedMoneyCollection(categories);
        }
    }
}
