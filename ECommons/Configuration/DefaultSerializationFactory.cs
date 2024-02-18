using Newtonsoft.Json;

namespace ECommons.Configuration;
/// <summary>
/// Extend this class and override existing methods to create your own serialization factory.
/// </summary>
public class DefaultSerializationFactory : ISerializationFactory
{
    public virtual string DefaultConfigFileName => "DefaultConfig.json";

    /// <summary>
    /// Deserialization method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inputData"></param>
    /// <returns></returns>
    public virtual T? Deserialize<T>(string inputData)
    {
        return JsonConvert.DeserializeObject<T>(inputData, new JsonSerializerSettings()
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace,
        });
    }

    /// <summary>
    /// Serialization method
    /// </summary>
    /// <param name="config"></param>
    /// <param name="prettyPrint">A parameter that informs serializar that pretty-print should be used, if possible.</param>
    /// <returns></returns>
    public virtual string Serialize(object config, bool prettyPrint)
    {
        return JsonConvert.SerializeObject(config, new JsonSerializerSettings()
        {
            Formatting = prettyPrint ? Formatting.Indented : Formatting.None,
            DefaultValueHandling = config.GetType().IsDefined(typeof(IgnoreDefaultValueAttribute), false) ? DefaultValueHandling.Ignore : DefaultValueHandling.Include
        });
    }
}
