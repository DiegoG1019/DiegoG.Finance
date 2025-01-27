using DiegoG.Finance.Serialization.MessagePackFormatters;
using MessagePack;
using MessagePack.Formatters;
using Newtonsoft.Json.Linq;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.InteropServices;

[assembly: MessagePackKnownFormatter(typeof(LabeledAmountFormatter))]

namespace DiegoG.Finance.Serialization.MessagePackFormatters;

public sealed class LabeledAmountFormatter : IMessagePackFormatter<LabeledAmount?>
{
    private LabeledAmountFormatter() { }

    public static readonly IMessagePackFormatter<LabeledAmount?> Instance = new LabeledAmountFormatter();

    public void Serialize(ref MessagePackWriter writer, LabeledAmount? value, MessagePackSerializerOptions options)
    {
        if (value == null)
            writer.WriteNil();
        else
        {
            decimal val = value.Amount;
            var values = MemoryMarshal.Cast<decimal, long>(MemoryMarshal.CreateSpan(ref val, 1));
            Debug.Assert(values.Length == 2);

            writer.Write(values[0]);
            writer.Write(values[1]);
            writer.Write(value.Label);
        }
    }

    public LabeledAmount? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.IsNil) return null;

        Span<long> values = stackalloc long[2]
        {
            reader.ReadInt64(),
            reader.ReadInt64()
        };

        var label = reader.ReadString();
        return new LabeledAmount(label ?? "?", MemoryMarshal.Cast<long, decimal>(values)[0]);
    }
}
