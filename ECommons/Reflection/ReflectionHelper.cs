using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Reflection
{
    public static class ReflectionHelper
    {
        const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public static object GetFoP(this object obj, string name)
        {
            return obj.GetType().GetField(name, AllFlags)?.GetValue(obj) 
                ?? obj.GetType().GetProperty(name, AllFlags)?.GetValue(obj);
        }

        public static void SetFoP(this object obj, string name, object value)
        {
            var field = obj.GetType().GetField(name, AllFlags);
            if(field != null)
            {
                field.SetValue(obj, value);
            }
            else
            {
                obj.GetType().GetProperty(name, AllFlags).SetValue(obj, value);
            }
        }

        public static object Call(this object obj, string name, params object[] values)
        {
            var info = obj.GetType().GetMethod(name, AllFlags, values.Select(x => x.GetType()).ToArray());
            return info.Invoke(obj, values);
        }
    }
}
