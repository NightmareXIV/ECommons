using ECommons.Logging;
using ECommons.Reflection;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace ECommons.Configuration;
/// <summary>
/// Extend this class and override existing methods to create your own serialization factory.
/// </summary>
public class DefaultSerializationFactory : ISerializationFactory
{
    public virtual string DefaultConfigFileName => "DefaultConfig.json";
    public virtual bool IsBinary => false;

    /// <summary>
    /// Deserialization method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inputData"></param>
    /// <returns></returns>
    public virtual T? Deserialize<T>(string inputData)
    {
        var type = typeof(T).GetFieldPropertyUnion("JsonSerializerSettings", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        JsonSerializerSettings settings;
        if(type != null && type.GetValue(null) is JsonSerializerSettings s)
        {
            settings = s;
            PluginLog.Verbose($"Using JSON serializer settings from object to perform deserialization");
        }
        else
        {
            settings = new JsonSerializerSettings()
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
            };
        }
        return JsonConvert.DeserializeObject<T>(inputData, settings);
    }

    public virtual T? Deserialize<T>(byte[] inputData)
    {
        return Deserialize<T>(Encoding.UTF8.GetString(inputData));
    }

    /// <summary>
    /// Serialization method
    /// </summary>
    /// <param name="config"></param>
    /// <param name="prettyPrint">A parameter that informs serializar that pretty-print should be used, if possible.</param>
    /// <returns></returns>
    public virtual string Serialize(object config, bool prettyPrint)
    {
        var type = config.GetType().GetFieldPropertyUnion("JsonSerializerSettings", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        JsonSerializerSettings settings;
        if(type != null && type.GetValue(null) is JsonSerializerSettings s)
        {
            settings = s;
            PluginLog.Verbose($"Using JSON serializer settings from object to perform serialization");
        }
        else
        {
            settings = new JsonSerializerSettings()
            {
                Formatting = prettyPrint ? Formatting.Indented : Formatting.None,
                DefaultValueHandling = config.GetType().IsDefined(typeof(IgnoreDefaultValueAttribute), false) ? DefaultValueHandling.Ignore : DefaultValueHandling.Include
            };
        }
        return JsonConvert.SerializeObject(config, settings);
    }

    public string Serialize(object config)
    {
        return Serialize(config, false);
    }

    public virtual byte[] SerializeAsBin(object config)
    {
        return Encoding.UTF8.GetBytes(Serialize(config));
    }
}
