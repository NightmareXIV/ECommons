using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.Configuration;
internal static class EzConfigValueStorage
{
    internal static ISerializationFactory DefaultSerializationFactory = new DefaultSerializationFactory();
}
