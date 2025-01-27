using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiegoG.Finance.Serialization.JsonConverters;

public class SpendingTrackerSheetConverter : JsonConverter<SpendingTrackerSheet>
{
    private SpendingTrackerSheetConverter() { }

    public static SpendingTrackerSheetConverter JsonConverter { get; } = new();

    public override SpendingTrackerSheet? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var buffer = JsonSerializer.Deserialize<SpendingTrackerSheetBuffer>(ref reader, options);
        return new SpendingTrackerSheet(
            new MoneyCollection(buffer.IncomeSources),
            new CategorizedMoneyCollection(buffer.ExpenseCategories)
        );
    }

    public override void Write(Utf8JsonWriter writer, SpendingTrackerSheet value, JsonSerializerOptions options)
    {
        var buffer = new SpendingTrackerSheetBuffer()
        {
            ExpenseCategories = value.ExpenseCategories._categories,
            IncomeSources = value.IncomeSources._moneylist
        };
        JsonSerializer.Serialize(writer, buffer, options);
    }
}
