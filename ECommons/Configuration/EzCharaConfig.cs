using ECommons.DalamudServices;
using ECommons.EzEventManager;
using ECommons.GameHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace ECommons.Configuration;

public class EzCharaConfig<T> where T : new()
{
    private string DefaultCharaConfigFileName => $"{Prefix}{0:X16}.json";
    public string DefaultCharaConfigFile => Path.Combine(EzConfig.GetPluginConfigDirectory(), DefaultCharaConfigFileName);

    private string CurrentCharaConfigFileName => $"{Prefix}{Player.CID:X16}.json";

    private string CharaConfigFileName(ulong CID) => $"{Prefix}{CID:X16}.json";
    public string CurrentCharaConfigFile => Path.Combine(EzConfig.GetPluginConfigDirectory(), CurrentCharaConfigFileName);

    private Dictionary<ulong, T> Cache = [];
    private Option[] Options;
    private string Prefix;

    public EzCharaConfig(IEnumerable<Option>? options = null, string prefix = "EzConfig")
    {
        Options = options?.ToArray() ?? [];
        Prefix = prefix;
        new EzLogout(() => SaveAll(Options.Contains(Option.UnloadOnLogout)));
    }

    public T Get() => Get(Player.CID);
    public T GetDefault() => Get(0);

    public bool TryGet(ulong CID, [NotNullWhen(true)] out T? value)
    {
        if(CID == 0 && !Options.Contains(Option.AllowDefaultConfig))
        {
            value = default;
            return false;
        }
        value = Get(CID);
        return true;
    }

    public T Get(ulong CID)
    {
        if(CID == 0)
        {
            if(!Options.Contains(Option.AllowDefaultConfig)) throw new InvalidOperationException("Player is not currently logged in");
        }
        if(Cache.TryGetValue(CID, out var result))
        {
            return result;
        }
        else
        {
            var ret = EzConfig.LoadConfiguration<T>(CharaConfigFileName(CID));
            Cache.Add(CID, ret);
            return ret;
        }
    }

    public void Save(ulong cid, bool unload = false)
    {
        if(Cache.TryGetValue(cid, out var val))
        {
            EzConfig.SaveConfiguration(val, CharaConfigFileName(cid), !Options.Contains(Option.NotIndented));
            if(unload) Cache.Remove(cid);
        }
    }

    public void SaveAll(bool unload = false)
    {
        foreach(var x in Cache.Keys.ToArray()) Save(x, unload);
    }

    public enum Option { AllowDefaultConfig, UnloadOnLogout, NotIndented }
}
