using Dalamud;
using ECommons.Logging;
using ECommons.DalamudServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.LanguageHelpers
{
    public static class Localization
    {
        public static string PararmeterSymbol = "??";
        public static string Separator = "==";
        internal static Dictionary<string, string> CurrentLocalization = new();
        public static bool Logging = false;

        public static void Init(ClientLanguage? lang = null)
        {
            CurrentLocalization.Clear();
            var file = lang == null ? GetLocFileLocation() : GetLocFileLocation(lang.Value);
            if (File.Exists(file))
            {
                var text = File.ReadAllText(file, Encoding.UTF8);
                var list = text.Replace("\r\n", "\n").Replace("\r", "").Split("\n");
                for(var i = 0;i<list.Length;i++)
                {
                    var x = list[i].Replace("\\n", "\n");
                    var e = x.Split(Separator);
                    if(e.Length == 2)
                    {
                        if (CurrentLocalization.ContainsKey(e[0]))
                        {
                            PluginLog.Warning($"[Localization] Duplicate localization entry {e[0]} found in localization file {file}");
                        }
                        CurrentLocalization[e[0]] = e[1];
                    }
                    else
                    {
                        PluginLog.Warning($"[Localization] Invalid entry {x} found in localization file {file}");
                    }
                }
                PluginLog.Information($"[Localization] Loaded {CurrentLocalization.Count} entries");
            }
            else
            {
                PluginLog.Information($"[Localization] Requested file {file} does not exists");
            }
        }

        public static void Save(ClientLanguage? lang = null)
        {
            var file = GetLocFileLocation(lang);
            File.WriteAllText(file, CurrentLocalization.Select(x => $"{x.Key.Replace("\n", "\\n")}{Separator}{x.Value.Replace("\n", "\\n")}").Join("\n"));
        }

        public static string GetLocFileLocation(ClientLanguage? lang = null)
        {
            return lang == null?
                Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, $"Language{(int)Svc.Data.Language}.ini")
                :Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, $"Language{(int)lang.Value}.ini");
        }

        public static string Loc(string s)
        {
            return s.Loc();
        }

        public static string Loc(string s, params object[] values)
        {
            return s.Loc(values);
        }
    }
}
