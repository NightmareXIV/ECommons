using Dalamud.Plugin.Ipc;
using Dalamud.Plugin.Ipc.Exceptions;
using System;
using System.Reflection;

namespace ECommons.EzIpcManager;
#nullable disable
internal static class SafeWrapperAny
{
    internal class Wrapper<T1, T2, T3, T4, T5, T6, T7, T8, TRet>()
    {
        [Obfuscation] internal Action<T1, T2, T3, T4, T5, T6, T7, T8> Action;
        [Obfuscation] internal Func<T1, T2, T3, T4, T5, T6, T7, T8, TRet> Function;
        [Obfuscation] internal ICallGateSubscriber Subscriber;

        [Obfuscation]
        internal void InvokeAction(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
        {
            try
            {

                if(Subscriber?.HasAction == false)
                {
                    return;
                }

                Action(a1, a2, a3, a4, a5, a6, a7, a8);
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }
        [Obfuscation]
        internal TRet InvokeFunction(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
        {
            try
            {

                return Subscriber?.HasFunction == false ? default : Function(a1, a2, a3, a4, a5, a6, a7, a8);
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }

    internal class Wrapper<T1, T2, T3, T4, T5, T6, T7, TRet>()
    {
        [Obfuscation] internal Action<T1, T2, T3, T4, T5, T6, T7> Action;
        [Obfuscation] internal Func<T1, T2, T3, T4, T5, T6, T7, TRet> Function;
        [Obfuscation] internal ICallGateSubscriber Subscriber;

        [Obfuscation]
        internal void InvokeAction(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
        {
            try
            {

                if(Subscriber?.HasAction == false)
                {
                    return;
                }

                Action(a1, a2, a3, a4, a5, a6, a7);
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }

        [Obfuscation]
        internal TRet InvokeFunction(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
        {
            try
            {

                return Subscriber?.HasFunction == false ? default : Function(a1, a2, a3, a4, a5, a6, a7);
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }

    internal class Wrapper<T1, T2, T3, T4, T5, T6, TRet>()
    {
        [Obfuscation] internal Action<T1, T2, T3, T4, T5, T6> Action;
        [Obfuscation] internal Func<T1, T2, T3, T4, T5, T6, TRet> Function;
        [Obfuscation] internal ICallGateSubscriber Subscriber;

        [Obfuscation]
        internal void InvokeAction(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
        {
            try
            {

                if(Subscriber?.HasAction == false)
                {
                    return;
                }

                Action(a1, a2, a3, a4, a5, a6);
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }

        [Obfuscation]
        internal TRet InvokeFunction(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
        {
            try
            {

                return Subscriber?.HasFunction == false ? default : Function(a1, a2, a3, a4, a5, a6);
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }

    internal class Wrapper<T1, T2, T3, T4, T5, TRet>()
    {
        [Obfuscation] internal Action<T1, T2, T3, T4, T5> Action;
        [Obfuscation] internal Func<T1, T2, T3, T4, T5, TRet> Function;
        [Obfuscation] internal ICallGateSubscriber Subscriber;

        [Obfuscation]
        internal void InvokeAction(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
        {
            try
            {

                if(Subscriber?.HasAction == false)
                {
                    return;
                }

                Action(a1, a2, a3, a4, a5);
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }

        [Obfuscation]
        internal TRet InvokeFunction(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
        {
            try
            {

                return Subscriber?.HasFunction == false ? default : Function(a1, a2, a3, a4, a5);
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }

    internal class Wrapper<T1, T2, T3, T4, TRet>()
    {
        [Obfuscation] internal Action<T1, T2, T3, T4> Action;
        [Obfuscation] internal Func<T1, T2, T3, T4, TRet> Function;
        [Obfuscation] internal ICallGateSubscriber Subscriber;

        [Obfuscation]
        internal void InvokeAction(T1 a1, T2 a2, T3 a3, T4 a4)
        {
            try
            {

                if(Subscriber?.HasAction == false)
                {
                    return;
                }

                Action(a1, a2, a3, a4);
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }

        [Obfuscation]
        internal TRet InvokeFunction(T1 a1, T2 a2, T3 a3, T4 a4)
        {
            try
            {

                return Subscriber?.HasFunction == false ? default : Function(a1, a2, a3, a4);
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }

    internal class Wrapper<T1, T2, T3, TRet>()
    {
        [Obfuscation] internal Action<T1, T2, T3> Action;
        [Obfuscation] internal Func<T1, T2, T3, TRet> Function;
        [Obfuscation] internal ICallGateSubscriber Subscriber;

        [Obfuscation]
        internal void InvokeAction(T1 a1, T2 a2, T3 a3)
        {
            try
            {

                if(Subscriber?.HasAction == false)
                {
                    return;
                }

                Action(a1, a2, a3);
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }

        [Obfuscation]
        internal TRet InvokeFunction(T1 a1, T2 a2, T3 a3)
        {
            try
            {

                return Subscriber?.HasFunction == false ? default : Function(a1, a2, a3);
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }

    internal class Wrapper<T1, T2, TRet>()
    {
        [Obfuscation] internal Action<T1, T2> Action;
        [Obfuscation] internal Func<T1, T2, TRet> Function;
        [Obfuscation] internal ICallGateSubscriber Subscriber;

        [Obfuscation]
        internal void InvokeAction(T1 a1, T2 a2)
        {
            try
            {

                if(Subscriber?.HasAction == false)
                {
                    return;
                }

                Action(a1, a2);
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }

        [Obfuscation]
        internal TRet InvokeFunction(T1 a1, T2 a2)
        {
            try
            {

                return Subscriber?.HasFunction == false ? default : Function(a1, a2);
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }

    internal class Wrapper<T1, TRet>()
    {
        [Obfuscation] internal Action<T1> Action;
        [Obfuscation] internal Func<T1, TRet> Function;
        [Obfuscation] internal ICallGateSubscriber Subscriber;

        [Obfuscation]
        internal void InvokeAction(T1 a1)
        {
            try
            {

                if(Subscriber?.HasAction == false)
                {
                    return;
                }

                Action(a1);
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }

        [Obfuscation]
        internal TRet InvokeFunction(T1 a1)
        {
            try
            {

                return Subscriber?.HasFunction == false ? default : Function(a1);
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }

    internal class Wrapper<TRet>()
    {
        [Obfuscation] internal Action Action;
        [Obfuscation] internal Func<TRet> Function;
        [Obfuscation] internal ICallGateSubscriber Subscriber;

        [Obfuscation]
        internal void InvokeAction()
        {
            try
            {

                if(Subscriber?.HasAction == false)
                {
                    return;
                }

                Action();
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
        }

        [Obfuscation]
        internal TRet InvokeFunction()
        {
            try
            {

                return Subscriber?.HasFunction == false ? default : Function();
            }
            catch(Exception e)
            {
                EzIPC.InvokeOnSafeInvocationException(e);
            }
            return default;
        }
    }
}
