using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiegoG.Finance.Serialization.JsonConverters;

public static class JsonConverterHelpers
{
    public static void AddFinanceConverters(this JsonSerializerOptions options)
        => options.Converters.AddFinanceConverters();

    public static void AddFinanceConverters(this IList<JsonConverter> converters)
    {
        converters.Add(MoneyCollectionConverter.JsonConverter);
        converters.Add(CurrencyConverter.JsonConverter);
        converters.Add(CategorizedMoneyCollectionConverter.JsonConverter);
    }
}

//public class CategorizedFinancialCollectionConverter : JsonConverterFactory
//{
//    private CategorizedFinancialCollectionConverter() { }

//    public static CategorizedFinancialCollectionConverter JsonConverter { get; } = new();

//    [ThreadStatic]
//    private static Type[]? GenericParamBuffer;

//    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
//    {
//        while (typeToConvert is not null)
//        {
//            if (typeToConvert.IsConstructedGenericType)
//            {
//                var definition = typeToConvert.GetGenericTypeDefinition();
//                if (definition == typeof(CategorizedFinancialCollection<>))
//                {
//                    var param = typeToConvert.GetGenericArguments()[0];
//                    GenericParamBuffer ??= new Type[2];
//                    GenericParamBuffer[0] = typeToConvert;
//                    GenericParamBuffer[1] = 
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
