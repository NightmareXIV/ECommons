using Dalamud.Plugin.Ipc.Exceptions;
using System;

namespace ECommons.EzIpcManager;
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
public static class EzIPCExtensions
{
    /// <summary>
    /// Tries to invoke previously obtained action delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
    {
        try
        {
            action(a1, a2, a3, a4, a5, a6, a7, a8);
            return true;
        }
        catch (IpcNotReadyError)
        {
            return false;
        }
    }
    /// <summary>
    /// Tries to invoke previously obtained action delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> action, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
    {
        try
        {
            action(a1, a2, a3, a4, a5, a6, a7);
            return true;
        }
        catch (IpcNotReadyError)
        {
            return false;
        }
    }
    /// <summary>
    /// Tries to invoke previously obtained action delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> action, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
    {
        try
        {
            action(a1, a2, a3, a4, a5, a6);
            return true;
        }
        catch (IpcNotReadyError)
        {
            return false;
        }
    }
    /// <summary>
    /// Tries to invoke previously obtained action delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> action, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
    {
        try
        {
            action(a1, a2, a3, a4, a5);
            return true;
        }
        catch (IpcNotReadyError)
        {
            return false;
        }
    }
    /// <summary>
    /// Tries to invoke previously obtained action delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 a1, T2 a2, T3 a3, T4 a4)
    {
        try
        {
            action(a1, a2, a3, a4);
            return true;
        }
        catch (IpcNotReadyError)
        {
            return false;
        }
    }
    /// <summary>
    /// Tries to invoke previously obtained action delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<T1, T2, T3>(this Action<T1, T2, T3> action, T1 a1, T2 a2, T3 a3)
    {
        try
        {
            action(a1, a2, a3);
            return true;
        }
        catch (IpcNotReadyError)
        {
            return false;
        }
    }
    /// <summary>
    /// Tries to invoke previously obtained action delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<T1, T2>(this Action<T1, T2> action, T1 a1, T2 a2)
    {
        try
        {
            action(a1, a2);
            return true;
        }
        catch (IpcNotReadyError)
        {
            return false;
        }
    }
    /// <summary>
    /// Tries to invoke previously obtained action delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<T1>(this Action<T1> action, T1 a1)
    {
        try
        {
            action(a1);
            return true;
        }
        catch (IpcNotReadyError)
        {
            return false;
        }
    }
    /// <summary>
    /// Tries to invoke previously obtained action delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke(this Action action)
    {
        try
        {
            action();
            return true;
        }
        catch (IpcNotReadyError)
        {
            return false;
        }
    }

    /// <summary>
    /// Tries to invoke previously obtained function delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <param name="ret">Result returned by a function if invocation succeeded</param>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<T1, T2, T3, T4, T5, T6, T7, T8, TRet>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, TRet> function, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, out TRet? ret)
    {
        try
        {
            ret = function(a1, a2, a3, a4, a5, a6, a7, a8);
            return true;
        }
        catch (IpcNotReadyError)
        {
            ret = default;
            return false;
        }
    }
    /// <summary>
    /// Tries to invoke previously obtained function delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <param name="ret">Result returned by a function if invocation succeeded</param>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<T1, T2, T3, T4, T5, T6, T7, TRet>(this Func<T1, T2, T3, T4, T5, T6, T7, TRet> function, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, out TRet? ret)
    {
        try
        {
            ret = function(a1, a2, a3, a4, a5, a6, a7);
            return true;
        }
        catch (IpcNotReadyError)
        {
            ret = default;
            return false;
        }
    }
    /// <summary>
    /// Tries to invoke previously obtained function delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <param name="ret">Result returned by a function if invocation succeeded</param>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<T1, T2, T3, T4, T5, T6, TRet>(this Func<T1, T2, T3, T4, T5, T6, TRet> function, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, out TRet? ret)
    {
        try
        {
            ret = function(a1, a2, a3, a4, a5, a6);
            return true;
        }
        catch (IpcNotReadyError)
        {
            ret = default;
            return false;
        }
    }
    /// <summary>
    /// Tries to invoke previously obtained function delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <param name="ret">Result returned by a function if invocation succeeded</param>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<T1, T2, T3, T4, T5, TRet>(this Func<T1, T2, T3, T4, T5, TRet> function, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, out TRet? ret)
    {
        try
        {
            ret = function(a1, a2, a3, a4, a5);
            return true;
        }
        catch (IpcNotReadyError)
        {
            ret = default;
            return false;
        }
    }
    /// <summary>
    /// Tries to invoke previously obtained function delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <param name="ret">Result returned by a function if invocation succeeded</param>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<T1, T2, T3, T4, TRet>(this Func<T1, T2, T3, T4, TRet> function, T1 a1, T2 a2, T3 a3, T4 a4, out TRet? ret)
    {
        try
        {
            ret = function(a1, a2, a3, a4);
            return true;
        }
        catch (IpcNotReadyError)
        {
            ret = default;
            return false;
        }
    }
    /// <summary>
    /// Tries to invoke previously obtained function delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <param name="ret">Result returned by a function if invocation succeeded</param>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<T1, T2, T3, TRet>(this Func<T1, T2, T3, TRet> function, T1 a1, T2 a2, T3 a3, out TRet? ret)
    {
        try
        {
            ret = function(a1, a2, a3);
            return true;
        }
        catch (IpcNotReadyError)
        {
            ret = default;
            return false;
        }
    }
    /// <summary>
    /// Tries to invoke previously obtained function delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <param name="ret">Result returned by a function if invocation succeeded</param>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<T1, T2, TRet>(this Func<T1, T2, TRet> function, T1 a1, T2 a2, out TRet? ret)
    {
        try
        {
            ret = function(a1, a2);
            return true;
        }
        catch (IpcNotReadyError)
        {
            ret = default;
            return false;
        }
    }
    /// <summary>
    /// Tries to invoke previously obtained function delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <param name="ret">Result returned by a function if invocation succeeded</param>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<T1, TRet>(this Func<T1, TRet> function, T1 a1, out TRet? ret)
    {
        try
        {
            ret = function(a1);
            return true;
        }
        catch (IpcNotReadyError)
        {
            ret = default;
            return false;
        }
    }
    /// <summary>
    /// Tries to invoke previously obtained function delegate via <see cref="EzIPC"/> safely. Does not throws <see cref="IpcNotReadyError"/>.
    /// </summary>
    /// <param name="ret">Result returned by a function if invocation succeeded</param>
    /// <returns>Whether invocation did not throw <see cref="IpcNotReadyError"/></returns>
    public static bool TryInvoke<TRet>(this Func<TRet> function, out TRet? ret)
    {
        try
        {
            ret = function();
            return true;
        }
        catch (IpcNotReadyError)
        {
            ret = default;
            return false;
        }
    }
}
