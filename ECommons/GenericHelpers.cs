using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons
{
    public static class GenericHelpers
    {
        public static string Repeat(this string s, int num)
        {
            StringBuilder str = new();
            for(var i = 0; i < num; i++)
            {
                str.Append(s);
            }
            return str.ToString();
        }

        public static void Safe(Action a)
        {
            try
            {
                a();
            }
            catch (Exception e)
            {
                PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
            }
        }

        public static bool TryExecute(Action a)
        {
            try
            {
                a();
                return true;
            }
            catch (Exception e)
            {
                PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
                return false;
            }
        }

        public static bool TryExecute<T>(Func<T> a, out T result)
        {
            try
            {
                result = a();
                return true;
            }
            catch (Exception e)
            {
                PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
                result = default;
                return false;
            }
        }

        public static bool EqualsAny<T>(this T obj, params T[] values)
        {
            return values.Any(x => x.Equals(obj));
        }

        public static bool TryGetFirst<K, V>(this IDictionary<K, V> dictionary, Func<KeyValuePair<K, V>, bool> predicate, out KeyValuePair<K, V> keyValuePair)
        {
            try
            {
                keyValuePair = dictionary.First(predicate);
                return true;
            }
            catch(Exception)
            {
                keyValuePair = default;
                return false;
            }
        }
    }
}
