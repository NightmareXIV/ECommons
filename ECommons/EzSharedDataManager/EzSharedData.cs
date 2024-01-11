using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ECommons.EzSharedDataManager;
#nullable disable

/// <summary>
/// EzSharedData class aims to resolve following problems with built-in SharedData service.
/// - It will automatically relinquish data upon plugin disposal, unless specifically instructed otherwise
/// - You can use TryGet continuously and there will ever be only one usage instance, you do not have to worry about holding reference yourself. Although, this is slower than creating and holding reference yourself so please obtain and hold reference to the data in performance-critical functions such as Framework Update.
/// </summary>
public static class EzSharedData
{
    internal static List<string> Keep = [];
    internal static Dictionary<string, object> Cache = [];
    /// <summary>
    /// Attempts to get existing data or create new data.
    /// </summary>
    /// <typeparam name="T">Data type. Note that only reference type works. If you need to use value type, use array and put one element there. Only .NET, Dalamud and Dalamud's libraries types; no submodules, nuget packages and your own custom-defined types. If you need to share complex structure, please use tuples and Alias any type feature https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-12.0/using-alias-types</typeparam>
    /// <param name="Name">Shared data name. Please prefix it with your plugin's name.</param>
    /// <param name="Data">Obtained reference.</param>
    /// <param name="Mode">Creation mode, if any is needed.</param>
    /// <param name="DefaultValue">Default value when creating data that doesn't exists.</param>
    /// <returns>Whether data could be obtained</returns>
    public static bool TryGet<T>(string Name, [NotNullWhen(true)] out T Data, CreationMode Mode = CreationMode.ReadOnly, T DefaultValue = null) where T : class
    {
        if (Cache.TryGetValue(Name, out var data))
        {
            Data = (T)data;
            return true;
        }
        if (Mode == CreationMode.ReadOnly)
        {
            if (Svc.PluginInterface.TryGetData<T>(Name, out Data))
            {
                Cache[Name] = Data;
                return true;
            }
        }
        else if (Mode == CreationMode.CreateAndRelinquish)
        {
            Data = Svc.PluginInterface.GetOrCreateData<T>(Name, () => DefaultValue);
            Cache[Name] = Data;
            return true;
        }
        else if (Mode == CreationMode.CreateAndKeep)
        {
            Data = Svc.PluginInterface.GetOrCreateData<T>(Name, () => DefaultValue);
            Keep.Add(Name);
            Cache[Name] = Data;
            return true;
        }
        Data = default;
        return false;
    }

    public static T GetOrCreate<T>(string Name, T DefaultValue = null) where T : class => Get(Name, CreationMode.CreateAndRelinquish, DefaultValue);

    public static T Get<T>(string Name, CreationMode Mode = CreationMode.ReadOnly, T DefaultValue = null) where T : class
    {
        if (Cache.TryGetValue(Name, out var data))
        {
            return (T)data;
        }
        if (Mode == CreationMode.ReadOnly)
        {
            if (Svc.PluginInterface.TryGetData<T>(Name, out var Data))
            {
                Cache[Name] = Data;
                return Data;
            }
            else
            {
                throw new SharedDataNotReadyException(Name);
            }
        }
        else if (Mode == CreationMode.CreateAndRelinquish)
        {
            var Data = Svc.PluginInterface.GetOrCreateData<T>(Name, () => DefaultValue);
            Cache[Name] = Data;
            return Data;
        }
        else if (Mode == CreationMode.CreateAndKeep)
        {
            var Data = Svc.PluginInterface.GetOrCreateData<T>(Name, () => DefaultValue);
            Keep.Add(Name);
            Cache[Name] = Data;
            return Data;
        }
        throw new ArgumentOutOfRangeException(nameof(Mode));
    }

    internal static void Dispose()
    {
        foreach(var x in Cache)
        {
            try
            {
                if (!Keep.Contains(x.Key))
                {
                    Svc.PluginInterface.RelinquishData(x.Key);
                }
            }
            catch(Exception e)
            {
                e.Log();
            }
        }
    }
}
