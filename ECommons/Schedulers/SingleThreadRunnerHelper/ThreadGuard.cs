namespace ECommons.Schedulers.SingleThreadRunnerHelper;
using System;

internal static class ThreadGuard
{
    [ThreadStatic] public static bool IsSingleThread;

    public static void EnsureNotBlocked()
    {
        if(IsSingleThread)
        {
            throw new InvalidOperationException("Blocking on Task (.Result, .Wait) is forbidden in SingleThreadRunner. Use .Sync() instead.");
        }
    }
}
