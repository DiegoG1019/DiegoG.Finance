using DiegoG.Finance.Serialization.MessagePackFormatters;
using MessagePack;
using MessagePack.Formatters;
using Newtonsoft.Json.Linq;
using NodaMoney;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: MessagePackKnownFormatter(typeof(CurrencyFormatter))]
[assembly: MessagePackKnownFormatter(typeof(WorkSheetPageInfoFormatter))]

namespace DiegoG.Finance.Serialization.MessagePackFormatters;

public sealed class WorkSheetPageInfoFormatter : IMessagePackFormatter<KeyValuePair<MonthlyPeriod, WorkSheetPage.Info>>
{
    private WorkSheetPageInfoFormatter() { }

    public static readonly IMessagePackFormatter<KeyValuePair<MonthlyPeriod, WorkSheetPage.Info>> Instance = new WorkSheetPageInfoFormatter();

    public void Serialize(ref MessagePackWriter writer, KeyValuePair<MonthlyPeriod, WorkSheetPage.Info> value, MessagePackSerializerOptions options)
    {
        writer.WriteInt32(value.Key.RawValue);
        MessagePackSerializer.Serialize(ref writer, value.Value, options);
    }

    public KeyValuePair<MonthlyPeriod, WorkSheetPage.Info> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var k = MonthlyPeriod.FromBinary(reader.ReadInt32());
        var v = MessagePackSerializer.Deserialize<WorkSheetPage.Info>(ref reader, options);
        return new(k, v);
    }
}

public sealed class CurrencyFormatter : IMessagePackFormatter<Currency>
{
    private CurrencyFormatter() { }

    public static readonly IMessagePackFormatter<Currency> Instance = new CurrencyFormatter();

    public void Serialize(ref MessagePackWriter writer, Currency value, MessagePackSerializerOptions options)
    {
        writer.Write(value.Code[0]);
        writer.Write(value.Code[1]);
        writer.Write(value.Code[2]);
    }

    public Currency Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        const string @namespace = "ISO-4217";

        Span<char> chars = stackalloc char[3]
        {
            reader.ReadChar(),
            reader.ReadChar(),
            reader.ReadChar()
        };

        return Currency.FromCode(new string(chars), @namespace);
    }
}
