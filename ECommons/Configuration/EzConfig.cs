using ECommons.Logging;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ECommons.Configuration;

public static class EzConfig
{
    public static IEzConfig Config { get; private set; }

    public static T Init<T>() where T : IEzConfig, new()
    {
        Config = LoadConfiguration<T>("DefaultConfig.json");
        return (T)Config;
    }

    public static void Save()
    {
        if (Config != null)
        {
            SaveConfiguration(Config, "DefaultConfig.json", true);
        }
    }

    public static void SaveConfiguration(this IEzConfig Configuration, string path, bool indented = false, bool appendConfigDirectory = true)
    {
        if (appendConfigDirectory) path = Path.Combine(Svc.PluginInterface.GetPluginConfigDirectory(), path);
        var antiCorruptionPath = $"{path}.new";
        if (File.Exists(antiCorruptionPath))
        {
            var saveTo = $"{antiCorruptionPath}.{DateTimeOffset.Now.ToUnixTimeMilliseconds()}";
            PluginLog.Warning($"Detected unsuccessfully saved file {antiCorruptionPath}: moving to {saveTo}");
            Notify.Warning("Detected unsuccessfully saved configuration file.");
            File.Move(antiCorruptionPath, saveTo);
            PluginLog.Warning($"Success. Please manually check {saveTo} file contents.");
        }
        PluginLog.Debug($"From caller {new StackTrace().GetFrames().Select(x => x.GetMethod()?.Name ?? "<unknown>").Join(" <- ")} engaging anti-corruption mechanism, writing file to {antiCorruptionPath}");
        File.WriteAllText(antiCorruptionPath, JsonConvert.SerializeObject(Configuration, new JsonSerializerSettings()
        {
            Formatting = indented ? Formatting.Indented : Formatting.None,
            DefaultValueHandling = Configuration.GetType().IsDefined(typeof(IgnoreDefaultValueAttribute), false) ?DefaultValueHandling.Ignore:DefaultValueHandling.Include
        }), Encoding.UTF8);
        PluginLog.Debug($"Now moving {antiCorruptionPath} to {path}");
        File.Move(antiCorruptionPath, path, true);
        PluginLog.Debug($"Configuration successfully saved.");
    }

    public static T LoadConfiguration<T>(string path, bool appendConfigDirectory = true) where T : IEzConfig, new()
    {
        if (appendConfigDirectory) path = Path.Combine(Svc.PluginInterface.GetPluginConfigDirectory(), path);
        if (!File.Exists(path))
        {
            return new T();
        }
        return JsonConvert.DeserializeObject<T>(File.ReadAllText(path, Encoding.UTF8), new JsonSerializerSettings()
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace,
        }) ?? new T();
    }
}
