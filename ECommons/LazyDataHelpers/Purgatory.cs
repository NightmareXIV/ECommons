using ECommons.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.LazyDataHelpers;
public static class Purgatory
{
    private static List<Action> Actions = [];
    private static List<Type> Types = [];

    public static void Add(Action action)
    {
        Actions.Add(action);
    }

    public static void Add(Type type)
    {
        Types.Add(type);
    }

    internal static void Purge()
    {
        foreach(var x in Actions)
        {
            try
            {
                x();
            }
            catch(Exception e)
            {
                e.Log();
            }
        }
        foreach(var x in Types)
        {
            try
            {
                var fpu = x.GetFieldPropertyUnions(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                foreach(var f in fpu)
                {
                    if(f.UnionType.IsClass)
                    {
                        try
                        {
                            f.SetValue(null, null);
                        }
                        catch(Exception e)
                        {
                            e.Log();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                e.Log();
            }
        }
        Actions = null!;
    }
}
