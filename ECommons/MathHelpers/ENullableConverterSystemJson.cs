using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECommons.MathHelpers;

public class ENullableConverterSystemJson : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(ENullable<>);
    }

    public override System.Text.Json.Serialization.JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type underlyingType = ENullable.GetUnderlyingType(typeToConvert)!;

        Type converterType = typeof(SystemTextJsonENullableConverterInner<>).MakeGenericType(underlyingType);

        return (System.Text.Json.Serialization.JsonConverter?)Activator.CreateInstance(converterType);
    }

    private class SystemTextJsonENullableConverterInner<T> : System.Text.Json.Serialization.JsonConverter<ENullable<T>> where T : struct
    {
        public override ENullable<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            
            if(reader.TokenType == JsonTokenType.Null)
            {
                return new ENullable<T>();
            }

            
            if(reader.TokenType == JsonTokenType.StartObject)
            {
                using(JsonDocument doc = JsonDocument.ParseValue(ref reader))
                {
                    JsonElement root = doc.RootElement;

                    
                    if(root.TryGetProperty("HasValue", out JsonElement hasValueElement) &&
                        root.TryGetProperty("Value", out JsonElement valueElement))
                    {
                        bool hasValue = hasValueElement.GetBoolean();
                        if(!hasValue)
                        {
                            return new ENullable<T>();
                        }

                        T value = System.Text.Json.JsonSerializer.Deserialize<T>(valueElement.GetRawText(), options);
                        return new ENullable<T>(value);
                    }

                    
                    T objValue = System.Text.Json.JsonSerializer.Deserialize<T>(root.GetRawText(), options);
                    return new ENullable<T>(objValue);
                }
            }

            
            T directValue = System.Text.Json.JsonSerializer.Deserialize<T>(ref reader, options);
            return new ENullable<T>(directValue);
        }

        public override void Write(Utf8JsonWriter writer, ENullable<T> value, JsonSerializerOptions options)
        {
            if(!value.HasValue)
            {
                writer.WriteNullValue();
            }
            else
            {
                System.Text.Json.JsonSerializer.Serialize(writer, value.Value, options);
            }
        }
    }
}
