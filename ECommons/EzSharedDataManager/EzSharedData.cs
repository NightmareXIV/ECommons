using ECommons.Logging;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.EzSharedDataManager
{
    public static class EzSharedData
    {
        internal static List<string> Keep = [];
        internal static Dictionary<string, object> Cache = [];
        public static bool TryGet<T>(string Name, out T Data, CreationMode Mode = CreationMode.ReadOnly, T DefaultValue = null) where T : class
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
            }
            else if (Mode == CreationMode.CreateAndKeep)
            {
                Data = Svc.PluginInterface.GetOrCreateData<T>(Name, () => DefaultValue);
                Keep.Add(Name);
                Cache[Name] = Data;
            }
            Data = default;
            return false;
        }

        /* This doesn't works!
         * public static bool TryGetValueType<T>(string Name, out T Data, CreationMode Mode = CreationMode.ReadOnly, T DefaultValue = default) where T:struct
        {
            if (Cache.TryGetValue(Name, out var data))
            {
                Data = ((T[])data)[0];
                return true;
            }
            if (Mode == CreationMode.ReadOnly)
            {
                if (Svc.PluginInterface.TryGetData<T[]>(Name, out var array))
                {
                    Cache[Name] = array;
                    Data = array[0];
                    return true;
                }
            }
            else if (Mode == CreationMode.CreateAndRelinquish)
            {
                Data = Svc.PluginInterface.GetOrCreateData<T[]>(Name, () => ([DefaultValue]))[0];
                Cache[Name] = Data;
                return true;
            }
            else if (Mode == CreationMode.CreateAndKeep)
            {
                Data = Svc.PluginInterface.GetOrCreateData<T[]>(Name, () => ([DefaultValue]))[0];
                Keep.Add(Name);
                Cache[Name] = Data;
                return true;
            }
            Data = default;
            return false;
        }*/

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
}
