using ECommons.Logging;
using Dalamud.Plugin;
using ECommons.DalamudServices;
using ECommons.Reflection;
using System;
using System.Linq;
using System.Reflection;

namespace ECommons.SplatoonAPI;

public static class Splatoon
{
    internal static IDalamudPlugin Instance;
    internal static int Version;

    internal static Action OnConnect;

    internal static void Init()
    {
        try
        {
            if (Svc.PluginInterface.GetIpcSubscriber<bool>("Splatoon.IsLoaded").InvokeFunc())
            {
                Connect();
            }
        }
        catch { }
        Svc.PluginInterface.GetIpcSubscriber<bool>("Splatoon.Loaded").Subscribe(Connect);
        Svc.PluginInterface.GetIpcSubscriber<bool>("Splatoon.Unloaded").Subscribe(Reset);
    }

    public static void SetOnConnect(Action action)
    {
        OnConnect = action;
        try
        {
            if (Svc.PluginInterface.GetIpcSubscriber<bool>("Splatoon.IsLoaded").InvokeFunc())
            {
                OnConnect();
            }
        }
        catch { }
    }

    internal static void Shutdown()
    {
        Svc.PluginInterface.GetIpcSubscriber<bool>("Splatoon.Loaded").Unsubscribe(Connect);
        Svc.PluginInterface.GetIpcSubscriber<bool>("Splatoon.Unloaded").Unsubscribe(Reset);
    }

    internal static void Reset()
    {
        Instance = null;
        PluginLog.Information("Disconnected from Splatoon");
    }

    static void Connect()
    {
        try
        {
            if (DalamudReflector.TryGetDalamudPlugin("Splatoon", out var plugin, false, true) && (bool)plugin.GetType().GetField("Init").GetValue(plugin))
            {
                Instance = plugin;
                Version++;
                OnConnect?.Invoke();
                PluginLog.Information("Successfully connected to Splatoon.");
            }
            else
            {
                throw new Exception("Splatoon is not initialized");
            }
        }
        catch (Exception e)
        {
            PluginLog.Error("Can't find Splatoon plugin: " + e.Message);
            PluginLog.Error(e.StackTrace);
        }
    }

    public static bool IsConnected()
    {
        return Instance != null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="e"></param>
    /// <param name="DestroyCondition">
    /// How much milliseconds to remove passed element.
    /// <br>0 = Don't auto-remove element.</br>
    /// <br>-1 = remove when exiting combat.</br>
    /// <br>-2 = remove when zone changes.</br>
    /// <br>If any of the conditions are met, the element will be automatically deleted.</br>
    /// </param>
    /// <returns></returns>
    public static bool AddDynamicElement(string name, Element e, long[] DestroyCondition)
    {
        return AddDynamicElements(name, new Element[] { e }, DestroyCondition);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="e"></param>
    /// <param name="DestroyCondition">
    /// How much milliseconds to remove passed element.
    /// <br>0 = Don't auto-remove element.</br>
    /// <br>-1 = remove when exiting combat.</br>
    /// <br>-2 = remove when zone changes.</br>
    /// </param>
    /// <returns></returns>
    public static bool AddDynamicElement(string name, Element e, long DestroyCondition)
    {
        return AddDynamicElements(name, new Element[] { e }, new long[] { DestroyCondition });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="e"></param>
    /// <param name="DestroyCondition">
    /// How much milliseconds to remove passed element.
    /// <br>0 = Don't auto-remove element.</br>
    /// <br>-1 = remove when exiting combat.</br>
    /// <br>-2 = remove when zone changes.</br>
    /// </param>
    /// <returns></returns>
    public static bool AddDynamicElements(string name, Element[] e, long DestroyCondition)
    {
        return AddDynamicElements(name, e, new long[] { DestroyCondition });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="e"></param>
    /// <param name="DestroyCondition">
    /// How much milliseconds to remove passed element.
    /// <br>0 = Don't auto-remove element.</br>
    /// <br>-1 = remove when exiting combat.</br>
    /// <br>-2 = remove when zone changes.</br>
    /// <br>If any of the conditions are met, the elements will be automatically deleted.</br>
    /// </param>
    /// <returns></returns>
    public static bool AddDynamicElements(string name, Element[] e, long[] DestroyCondition)
    {
        if (!IsConnected())
        {
            PluginLog.Warning("Not connected to Splatoon");
            return false;
        }
        if (!e.All(x => x.IsValid()))
        {
            PluginLog.Warning("Elements are no longer valid");
            return false;
        }
        if (e.Length == 0)
        {
            PluginLog.Warning("There are no elements");
            return false;
        }
        try
        {
            var array = Array.CreateInstance(e[0].Instance.GetType(), e.Length);
            for (var i = 0; i < e.Length; i++)
            {
                array.SetValue(e[i].Instance, i);
            }
            Instance.GetType().GetMethod("AddDynamicElements").Invoke(Instance, new object[] { name, array, DestroyCondition });
            return true;
        }
        catch (Exception ex)
        {
            ex.Log();
            return false;
        }
    }

    public static bool DisplayOnce(Element e)
    {
        if (!IsConnected())
        {
            PluginLog.Warning("Not connected to Splatoon");
            return false;
        }
        if (!e.IsValid())
        {
            PluginLog.Warning("Elements are no longer valid");
            return false;
        }
        try
        {
            Instance.GetType().GetMethod("InjectElement").Invoke(Instance, new object[] { e.Instance });
            return true;
        }
        catch (Exception ex)
        {
            ex.Log();
            return false;
        }
    }

    public static bool RemoveDynamicElements(string name)
    {
        if (!IsConnected())
        {
            PluginLog.Warning("Not connected to Splatoon");
            return false;
        }
        try
        {
            Instance.GetType().GetMethod("RemoveDynamicElements").Invoke(Instance, new object[] { name });
            return true;
        }
        catch (Exception ex)
        {
            ex.Log();
            return false;
        }
    }

    public static Element DecodeElement(string input)
    {
        var method = Instance.GetType().Assembly.GetType("Splatoon.SplatoonScripting.ScriptingEngine", true).GetMethod("TryDecodeElement", BindingFlags.Public | BindingFlags.Static);
        var parameters = new object[] { input, null };
        var result = (bool)method.Invoke(null, parameters);
        if (result)
        {
            return new Element(parameters[1]);
        }
        else
        {
            return null;
        }
    }

    [Obsolete("Work in progress")]
    public static object DecodeLayout(string input)
    {
        var method = Instance.GetType().Assembly.GetType("Splatoon.SplatoonScripting.ScriptingEngine", true).GetMethod("TryDecodeLayout", BindingFlags.Public | BindingFlags.Static);
        var parameters = new object[] { input, null };
        var result = (bool)method.Invoke(null, parameters);
        if (result)
        {
            return parameters[1];
        }
        else
        {
            return null;
        }
    }
}
