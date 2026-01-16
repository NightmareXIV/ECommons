using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommons.MathHelpers;

public class ENullableConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsGenericType &&
               objectType.GetGenericTypeDefinition() == typeof(ENullable<>);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        
        if(reader.TokenType == JsonToken.Null)
        {
            return Activator.CreateInstance(objectType);
        }

        
        Type underlyingType = ENullable.GetUnderlyingType(objectType)!;

        
        try
        {
            object? value;

            
            if(reader.TokenType == JsonToken.StartObject)
            {
                
                JObject obj = JObject.Load(reader);

                if(obj.ContainsKey("HasValue") && obj.ContainsKey("Value"))
                {
                    
                    bool hasValue = obj["HasValue"]!.Value<bool>();
                    if(!hasValue)
                    {
                        return Activator.CreateInstance(objectType);
                    }
                    value = obj["Value"]!.ToObject(underlyingType, serializer);
                }
                else
                {
                    
                    value = obj.ToObject(underlyingType, serializer);
                }
            }
            else
            {
                
                value = serializer.Deserialize(reader, underlyingType);
            }

            if(value == null)
            {
                return Activator.CreateInstance(objectType);
            }

            
            return Activator.CreateInstance(objectType, value);
        }
        catch
        {
            
            return Activator.CreateInstance(objectType);
        }
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if(value == null)
        {
            writer.WriteNull();
            return;
        }

        
        Type type = value.GetType();
        bool hasValue = (bool)type.GetProperty("HasValue")!.GetValue(value)!;

        if(!hasValue)
        {
            writer.WriteNull();
        }
        else
        {
            object actualValue = type.GetProperty("Value")!.GetValue(value)!;
            serializer.Serialize(writer, actualValue);
        }
    }
}
