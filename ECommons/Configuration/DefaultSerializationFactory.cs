using ECommons.ImGuiMethods;
using ECommons.Logging;
using ECommons.Reflection;
using Newtonsoft.Json;
using System;
using System.IO;
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

    public virtual byte[] ReadFileAsBin(string fullPath)
    {
        return File.ReadAllBytes(fullPath);
    }

    public virtual string ReadFileAsText(string fullPath)
    {
        return File.ReadAllText(fullPath, Encoding.UTF8);
    }

    public virtual bool FileExists(string fullPath)
    {
        return File.Exists(fullPath);
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

    public virtual void WriteFile(string fullPath, string data)
    {
        var antiCorruptionPath = $"{fullPath}.new";
        if(File.Exists(antiCorruptionPath))
        {
            var saveTo = $"{antiCorruptionPath}.{DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
            PluginLog.Warning($"Detected unsuccessfully saved file {antiCorruptionPath}: moving to {saveTo}");
            Notify.Warning("Detected unsuccessfully saved configuration file.");
            File.Move(antiCorruptionPath, saveTo);
            PluginLog.Warning($"Success. Please manually check {saveTo} file contents.");
        }
        File.WriteAllText(antiCorruptionPath, data, Encoding.UTF8);
        File.Move(antiCorruptionPath, fullPath, true);
    }

    public virtual void WriteFile(string fullPath, byte[] data)
    {
        var antiCorruptionPath = $"{fullPath}.new";
        if(File.Exists(antiCorruptionPath))
        {
            var saveTo = $"{antiCorruptionPath}.{DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
            PluginLog.Warning($"Detected unsuccessfully saved file {antiCorruptionPath}: moving to {saveTo}");
            Notify.Warning("Detected unsuccessfully saved configuration file.");
            File.Move(antiCorruptionPath, saveTo);
            PluginLog.Warning($"Success. Please manually check {saveTo} file contents.");
        }
        File.WriteAllBytes(antiCorruptionPath, data);
        File.Move(antiCorruptionPath, fullPath, true);
    }
}
