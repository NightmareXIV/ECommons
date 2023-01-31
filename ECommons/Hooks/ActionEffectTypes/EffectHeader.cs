using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PInvoke.User32;

namespace ECommons.Hooks.ActionEffectTypes
{
    [StructLayout(LayoutKind.Explicit)]
    public struct EffectHeader
    {
        [FieldOffset(8)] public uint ActionID;
        [FieldOffset(28)] public ushort AnimationId;
        [FieldOffset(33)] public byte TargetCount;
    }
}
