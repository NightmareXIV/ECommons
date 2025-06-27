using ECommons.Reflection;

namespace ECommons.WindowsFormsReflector;
public static partial class Winforms
{
    public static class Clipboard
    {
        public static object Instance
        {
            get
            {
                return ReflectionHelper.GetTypeFromRuntimeAssembly("System.Windows.Forms", "System.Windows.Forms.Clipboard");
            }
        }

        public static void Clear()
        {
            Instance.Call("Clear", []);
        }

        public static void SetText(string text)
        {
            Instance.Call("SetText", [text]);
        }

        public static string GetText()
        {
            return Instance.Call<string>("GetText", []);
        }
    }
}