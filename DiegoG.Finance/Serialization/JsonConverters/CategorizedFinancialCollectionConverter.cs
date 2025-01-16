using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DiegoG.Finance.Serialization.JsonConverters;

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
