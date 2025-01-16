using DiegoG.Finance.Serialization.MessagePackFormatters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiegoG.Finance.Serialization.JsonConverters;

public class MoneyCollectionConverter : JsonConverter<MoneyCollection>
{
    private MoneyCollectionConverter() { }

    public static MoneyCollectionConverter JsonConverter { get; } = new();

    public override MoneyCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var buffer = JsonSerializer.Deserialize<MoneyCollectionBuffer>(ref reader, options);
        return new MoneyCollection(buffer.Currency, buffer.AmountSet);
    }

    public override void Write(Utf8JsonWriter writer, MoneyCollection value, JsonSerializerOptions options)
    {
        var buffer = new MoneyCollectionBuffer()
        {
            Currency = value.Currency,
            AmountSet = value._moneylist
        };
        JsonSerializer.Serialize(writer, buffer, options);
    }
}
