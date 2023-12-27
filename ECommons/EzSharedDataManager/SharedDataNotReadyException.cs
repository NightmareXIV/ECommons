using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.EzSharedDataManager;
internal class SharedDataNotReadyException : Exception
{
    internal SharedDataNotReadyException(string dataName) : base($"Shared data tag {dataName} is not ready yet")
    {
    }
}
