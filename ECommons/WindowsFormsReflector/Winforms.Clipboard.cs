using ECommons.Reflection;
using System;
using System.Reflection;

namespace ECommons.WindowsFormsReflector;
public static partial class Winforms
{
    public static class Clipboard
    {
        public static Type Instance
        {
            get
            {
                return ReflectionHelper.GetTypeFromRuntimeAssembly("System.Windows.Forms", "System.Windows.Forms.Clipboard");
            }
        }

        public static void Clear()
        {
            Instance.GetMethod("Clear", BindingFlags.Public | BindingFlags.Static, []).Invoke(null, []);
        }

        public static void SetText(string text)
        {
            Instance.GetMethod("SetText", BindingFlags.Public | BindingFlags.Static, [typeof(string)]).Invoke(null, [text]);
        }

        public static string GetText()
        {
            return Instance.GetMethod("GetText", BindingFlags.Public | BindingFlags.Static, []).Invoke(null, []) as string;
        }
    }
}