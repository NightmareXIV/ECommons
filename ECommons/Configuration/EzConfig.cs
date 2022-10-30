using ECommons.DalamudServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Configuration
{
    public static class EzConfig
    {
        public static IEzConfig Config { get; private set; }

        public static T Init<T>() where T : IEzConfig, new()
        {
            Config = LoadConfiguration<T>(Path.Combine(Svc.PluginInterface.GetPluginConfigDirectory(), "DefaultConfig.json"));
            return (T)Config;
        }

        public static void Save()
        {
            if (Config != null)
            {
                SaveConfiguration(Config, Path.Combine(Svc.PluginInterface.GetPluginConfigDirectory(), "DefaultConfig.json"));
            }
        }

        public static void SaveConfiguration(this IEzConfig Configuration, string Path)
        {
            File.WriteAllText(Path, JsonConvert.SerializeObject(Configuration, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
            }), Encoding.UTF8);
        }

        public static T LoadConfiguration<T>(string Path) where T : IEzConfig, new()
        {
            if (!File.Exists(Path))
            {
                return new T();
            }
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(Path, Encoding.UTF8), new JsonSerializerSettings()
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
            }) ?? new T();
        }
    }
}
