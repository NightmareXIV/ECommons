using Dalamud;
using Dalamud.Logging;
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
                var list = text.ReplaceLineEndings().Split(Environment.NewLine);
                foreach(var x in list)
                {
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
                PluginLog.Information($"[Localization] Loaded {CurrentLocalization} entries");
            }
            else
            {
                PluginLog.Information($"[Localization] Requested file {file} does not exists");
            }
        }

        public static void Save(ClientLanguage? lang = null)
        {
            var file = GetLocFileLocation(lang);
            File.WriteAllText(file, CurrentLocalization.Select(x => $"{x.Key}{Separator}{x.Value}").Join(Environment.NewLine));
        }

        public static string GetLocFileLocation(ClientLanguage? lang = null)
        {
            return lang == null?
                Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, $"Language{(int)Svc.Data.Language}.ini")
                :Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, $"Language{(int)lang.Value}.ini");
        }

        public static string Loc(this string s)
        {
            if (CurrentLocalization.TryGetValue(s, out var locs) && locs != "" && locs != null)
            {
                return locs;
            }
            else if (Logging)
            {
                CurrentLocalization[s] = "";
            }
            return s;
        }

        public static string Loc(this string s, params object[] values)
        {
            if (CurrentLocalization.TryGetValue(s, out var locs) && locs != "" && locs != null)
            {
                foreach(var x in values)
                {
                    locs = locs.ReplaceFirst(PararmeterSymbol, x.ToString());
                }
                return locs;
            }
            else if (Logging)
            {
                CurrentLocalization[s] = "";
            }
            return s;
        }
    }
}
