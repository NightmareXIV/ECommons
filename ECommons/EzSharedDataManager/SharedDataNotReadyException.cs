using System;

namespace ECommons.EzSharedDataManager;
internal class SharedDataNotReadyException : Exception
{
    internal SharedDataNotReadyException(string dataName) : base($"Shared data tag {dataName} is not ready yet")
    {
    }
}
