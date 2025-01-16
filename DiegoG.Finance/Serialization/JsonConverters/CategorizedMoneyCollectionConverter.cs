using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiegoG.Finance.Serialization.JsonConverters;

public class CategorizedMoneyCollectionConverter : JsonConverter<CategorizedMoneyCollection>
{
    private CategorizedMoneyCollectionConverter() { }

    public static CategorizedMoneyCollectionConverter JsonConverter { get; } = new();

    public override CategorizedMoneyCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var buffer = JsonSerializer.Deserialize<CategorizedMoneyCollectionBuffer>(ref reader, options);
        return new CategorizedMoneyCollection(buffer.Currency, buffer.Categories);
    }

    public override void Write(Utf8JsonWriter writer, CategorizedMoneyCollection value, JsonSerializerOptions options)
    {
        var buffer = new CategorizedMoneyCollectionBuffer()
        {
            Currency = value.Currency,
            Categories = value._categories
        };
        JsonSerializer.Serialize(writer, buffer, options);
    }
}
