using ECommons.Logging;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace ECommons.Configuration;

/// <summary>
/// A class that aims to significantly simplify working with Dalamud configuration.
/// 1. Does not includes type definitions, which allows changing underlying type if it can be deserialized from existing data (list into array...)
/// 2. Provides anti-corruption mechanism, reducing chance of data loss if game crashes or power goes off during configuration writing
/// 3. Allows to very easily load default configuration as well as additional configuration, taking path to config folder into account.
/// 4. Allows you to redefine serializer with your own implementation upon serializing or in general for the whole EzConfig module.
/// 5. Solves the issues with default Dalamud serialization settings where default values of collection will stay in addition to ones that were deserialized.
/// </summary>
public static class EzConfig
{
    /// <summary>
    /// Full path to default configuration file.
    /// </summary>
    public static string DefaultConfigurationFileName => Path.Combine(Svc.PluginInterface.GetPluginConfigDirectory(), DefaultSerializationFactory.DefaultConfigFileName);
    /// <summary>
    /// Default configuration reference
    /// </summary>
    public static IEzConfig? Config { get; private set; }

    private static bool WasCalled = false;

    /// <summary>
    /// Default serialization factory. Create a class that extends SerializationFactory, implement your own serializer and deserializer and assign DefaultSerializationFactory to it before loading any configurations to change serializer to your own liking.
    /// </summary>
    public static ISerializationFactory DefaultSerializationFactory 
    {
        get
        {
            return EzConfigValueStorage.DefaultSerializationFactory;
        }
        set
        {
            if (WasCalled) throw new InvalidOperationException("Can not change DefaultSerializationFactory after any configurations has been loaded or saved");
            EzConfigValueStorage.DefaultSerializationFactory = value;
        }
    }

    /// <summary>
    /// Loads and returns default configuration file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Init<T>() where T : IEzConfig, new()
    {
        Config = LoadConfiguration<T>(DefaultSerializationFactory.DefaultConfigFileName);
        return (T)Config;
    }

    /// <summary>
    /// Migrates old default configuration to EzConfig, if applicable. Must be called before Init.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="NullReferenceException"></exception>
    public static void Migrate<T>() where T : IEzConfig, new()
    {
        if (Config != null)
        {
            throw new NullReferenceException("Migrate must be called before initialization");
        }
        WasCalled = true;
        var path = DefaultConfigurationFileName;
        if(!File.Exists(path) && Svc.PluginInterface.ConfigFile.Exists)
        {
            PluginLog.Warning($"Migrating {Svc.PluginInterface.ConfigFile} into EzConfig system");
            Config = LoadConfiguration<T>(Svc.PluginInterface.ConfigFile.FullName, false);
            Save();
            Config = null;
            File.Move(Svc.PluginInterface.ConfigFile.FullName, $"{Svc.PluginInterface.ConfigFile}.old");
        }
        else
        {
            PluginLog.Information($"Migrating conditions are not met, skipping...");
        }
    }

    /// <summary>
    /// Saves default configuration file, if applicable. 
    /// </summary>
    public static void Save()
    {
        if (Config != null)
        {
            SaveConfiguration(Config, DefaultSerializationFactory.DefaultConfigFileName, true);
        }
    }

    /// <summary>
    /// Saves arbitrary configuration file.
    /// </summary>
    /// <param name="Configuration">Configuration instance</param>
    /// <param name="path">Path to save to</param>
    /// <param name="prettyPrint">Inform serializer that you want pretty-print your configuration</param>
    /// <param name="appendConfigDirectory">If true, plugin configuration directory will be added to path</param>
    /// <param name="serializationFactory">If null, then default factory will be used.</param>
    public static void SaveConfiguration(this IEzConfig Configuration, string path, bool prettyPrint = false, bool appendConfigDirectory = true, ISerializationFactory? serializationFactory = null)
    {
        WasCalled = true;
        serializationFactory ??= DefaultSerializationFactory;
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
        PluginLog.Verbose($"From caller {new StackTrace().GetFrames().Select(x => x.GetMethod()?.Name ?? "<unknown>").Join(" <- ")} engaging anti-corruption mechanism, writing file to {antiCorruptionPath}");
        File.WriteAllText(antiCorruptionPath, serializationFactory.Serialize(Configuration, prettyPrint), Encoding.UTF8);
        PluginLog.Verbose($"Now moving {antiCorruptionPath} to {path}");
        File.Move(antiCorruptionPath, path, true);
        PluginLog.Verbose($"Configuration successfully saved.");
    }

    /// <summary>
    /// Loads arbitrary configuration file or creates an empty one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">Where to load it from.</param>
    /// <param name="appendConfigDirectory">If true, plugin configuration directory will be added to path</param>
    /// <param name="serializationFactory">If null, then default factory will be used.</param>
    /// <returns></returns>
    public static T LoadConfiguration<T>(string path, bool appendConfigDirectory = true, ISerializationFactory? serializationFactory = null) where T : IEzConfig, new()
    {
        WasCalled = true;
        serializationFactory ??= DefaultSerializationFactory;
        if (appendConfigDirectory) path = Path.Combine(Svc.PluginInterface.GetPluginConfigDirectory(), path);
        if (!File.Exists(path))
        {
            return new T();
        }
        return serializationFactory.Deserialize<T>(File.ReadAllText(path, Encoding.UTF8)) ?? new T();
    }
}
