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
    }
}
