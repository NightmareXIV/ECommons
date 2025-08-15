namespace ECommons.Schedulers.SingleThreadRunnerHelper;
using System;
using System.Threading.Tasks;

public static class SyncHelper
{
    [ThreadStatic] private static SingleThreadContext current;

    internal static void SetContext(SingleThreadContext ctx) => current = ctx;

    public static void Sync(this Task task)
    {
        if(current == null)
        {
            throw new InvalidOperationException("No SingleThreadContext is active.");
        }

        current.Run(() => task);
    }

    public static T Sync<T>(this Task<T> task)
    {
        if(current == null)
        {
            throw new InvalidOperationException("No SingleThreadContext is active.");
        }

        return current.Run(() => task);
    }
}
