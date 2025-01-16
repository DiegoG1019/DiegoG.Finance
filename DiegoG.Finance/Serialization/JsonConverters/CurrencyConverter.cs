using NodaMoney;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiegoG.Finance.Serialization.JsonConverters;

public class CurrencyConverter : JsonConverter<Currency>
{
    private CurrencyConverter() { }

    public static CurrencyConverter JsonConverter { get; } = new();

    const string @namespace = "ISO-4217";

    public override Currency Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var code = reader.GetString();
        return Currency.FromCode(code, @namespace);
    }

    public override void Write(Utf8JsonWriter writer, Currency value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Code);
    }
}

//public class CategorizedFinancialCollectionConverter<TCollectionType, TValueType> : JsonConverter<TCollectionType>
//{
//    public override TCollectionType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        var dict = JsonSerializer.Deserialize<Dictionary<string, TValueType>>(ref reader, options);
//        return Activator.CreateInstance(typeof(TCollectionType), true,)
//    }

//    public override void Write(Utf8JsonWriter writer, TCollectionType value, JsonSerializerOptions options)
//    {
//        throw new NotImplementedException();
//    }
//}

//public class CategorizedFinancialCollectionConverter : JsonConverterFactory
//{
//    private CategorizedFinancialCollectionConverter() { }

//    public static CategorizedFinancialCollectionConverter JsonConverter { get; } = new();

//    public override JsonConverter? CreateConverter([DisallowNull] Type? typeToConvert, JsonSerializerOptions options)
//    {
//        Type[] genericParams;
//        while (typeToConvert is not null)
//        {
//            if (typeToConvert.IsConstructedGenericType)
//            {
//                var definition = typeToConvert.GetGenericTypeDefinition();
//                if (definition == typeof(CategorizedFinancialCollection<>))
//                {
//                    genericParams = typeToConvert.GetGenericArguments();
//                    return;
//                }
//            }

//            typeToConvert = typeToConvert.BaseType;
//        }

//        return null;
//    }

//    public override bool CanConvert([DisallowNull] Type? typeToConvert)
//    {
//        while (typeToConvert is not null)
//        {
//            if (typeToConvert.IsConstructedGenericType)
//            {
//                var definition = typeToConvert.GetGenericTypeDefinition();
//                if (definition.IsAssignableTo(typeof(CategorizedFinancialCollection<>)))
//                    return true;
//            }

//            typeToConvert = typeToConvert.BaseType;
//        }

//        return false;
//    }
//}
