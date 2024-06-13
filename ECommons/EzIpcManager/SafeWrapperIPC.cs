using Dalamud.Plugin.Ipc.Exceptions;
using System;
using System.Reflection;

namespace ECommons.EzIpcManager;
#nullable disable
[Obfuscation(Exclude = true)]
internal static class SafeWrapperIPC
{
    internal class Wrapper<T1, T2, T3, T4, T5, T6, T7, T8, TRet>()
    {
        internal Action<T1, T2, T3, T4, T5, T6, T7, T8> Action;
        internal Func<T1, T2, T3, T4, T5, T6, T7, T8, TRet> Function;

        internal void InvokeAction(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
        {
            try
            {
                Action(a1, a2, a3, a4, a5, a6, a7, a8);
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }
        internal TRet InvokeFunction(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
        {
            try
            {
                return Function(a1, a2, a3, a4, a5, a6, a7, a8);
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }

    internal class Wrapper<T1, T2, T3, T4, T5, T6, T7, TRet>()
    {
        internal Action<T1, T2, T3, T4, T5, T6, T7> Action;
        internal Func<T1, T2, T3, T4, T5, T6, T7, TRet> Function;

        internal void InvokeAction(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
        {
            try
            {
                Action(a1, a2, a3, a4, a5, a6, a7);
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }

        internal TRet InvokeFunction(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
        {
            try
            {
                return Function(a1, a2, a3, a4, a5, a6, a7);
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }

    internal class Wrapper<T1, T2, T3, T4, T5, T6, TRet>()
    {
        internal Action<T1, T2, T3, T4, T5, T6> Action;
        internal Func<T1, T2, T3, T4, T5, T6, TRet> Function;

        internal void InvokeAction(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
        {
            try
            {
                Action(a1, a2, a3, a4, a5, a6);
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }

        internal TRet InvokeFunction(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
        {
            try
            {
                return Function(a1, a2, a3, a4, a5, a6);
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }

    internal class Wrapper<T1, T2, T3, T4, T5, TRet>()
    {
        internal Action<T1, T2, T3, T4, T5> Action;
        internal Func<T1, T2, T3, T4, T5, TRet> Function;

        internal void InvokeAction(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
        {
            try
            {
                Action(a1, a2, a3, a4, a5);
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }

        internal TRet InvokeFunction(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
        {
            try
            {
                return Function(a1, a2, a3, a4, a5);
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }

    internal class Wrapper<T1, T2, T3, T4, TRet>()
    {
        internal Action<T1, T2, T3, T4> Action;
        internal Func<T1, T2, T3, T4, TRet> Function;

        internal void InvokeAction(T1 a1, T2 a2, T3 a3, T4 a4)
        {
            try
            {
                Action(a1, a2, a3, a4);
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }

        internal TRet InvokeFunction(T1 a1, T2 a2, T3 a3, T4 a4)
        {
            try
            {
                return Function(a1, a2, a3, a4);
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }

    internal class Wrapper<T1, T2, T3, TRet>()
    {
        internal Action<T1, T2, T3> Action;
        internal Func<T1, T2, T3, TRet> Function;

        internal void InvokeAction(T1 a1, T2 a2, T3 a3)
        {
            try
            {
                Action(a1, a2, a3);
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }

        internal TRet InvokeFunction(T1 a1, T2 a2, T3 a3)
        {
            try
            {
                return Function(a1, a2, a3);
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }

    internal class Wrapper<T1, T2, TRet>()
    {
        internal Action<T1, T2> Action;
        internal Func<T1, T2, TRet> Function;

        internal void InvokeAction(T1 a1, T2 a2)
        {
            try
            {
                Action(a1, a2);
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }

        internal TRet InvokeFunction(T1 a1, T2 a2)
        {
            try
            {
                return Function(a1, a2);
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }

    internal class Wrapper<T1, TRet>()
    {
        internal Action<T1> Action;
        internal Func<T1, TRet> Function;

        internal void InvokeAction(T1 a1)
        {
            try
            {
                Action(a1);
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }

        internal TRet InvokeFunction(T1 a1)
        {
            try
            {
                return Function(a1);
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }

    internal class Wrapper<TRet>()
    {
        internal Action Action;
        internal Func<TRet> Function;

        internal void InvokeAction()
        {
            try
            {
                Action();
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }

        internal TRet InvokeFunction()
        {
            try
            {
                return Function();
            }
            catch (IpcNotReadyError e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }
}
