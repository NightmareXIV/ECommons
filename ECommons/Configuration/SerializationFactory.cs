using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Configuration;
/// <summary>
/// Extend this class and override existing methods to create your own serialization factory.
/// </summary>
public class SerializationFactory
{
    /// <summary>
    /// Deserialization method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inputData"></param>
    /// <returns></returns>
    public virtual T Deserialize<T>(string inputData) where T:IEzConfig
    {
        return JsonConvert.DeserializeObject<T>(inputData, new JsonSerializerSettings()
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace,
        });
    }

    /// <summary>
    /// Serialization method
    /// </summary>
    /// <param name="s"></param>
    /// <param name="prettyPrint">A parameter that informs serializar that pretty-print should be used, if possible.</param>
    /// <returns></returns>
    public virtual string Serialize(IEzConfig s, bool prettyPrint)
    {
        return JsonConvert.SerializeObject(s, new JsonSerializerSettings()
        {
            Formatting = prettyPrint ? Formatting.Indented : Formatting.None,
            DefaultValueHandling = s.GetType().IsDefined(typeof(IgnoreDefaultValueAttribute), false) ? DefaultValueHandling.Ignore : DefaultValueHandling.Include
        });
    }
}
