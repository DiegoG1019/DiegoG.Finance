using DiegoG.Finance.Serialization.MessagePackFormatters;
using MessagePack;
using MessagePack.Formatters;
using NodaMoney;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: MessagePackKnownFormatter(typeof(CurrencyFormatter))]

namespace DiegoG.Finance.Serialization.MessagePackFormatters;
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
