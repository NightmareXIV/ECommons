using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.EzHookManager
{
    public record HookFromSignatureOptions
    {
        public string Signature;
        public bool IsStatic = false;
        public nint Offset = 0;
        public int StaticOffset = 0;

        public HookFromSignatureOptions(string signature, nint offset = 0)
        {
            Signature = signature;
            Offset = offset;
        }

        public HookFromSignatureOptions(string signature, bool isStatic = false, nint offset = 0, int staticOffset = 0)
        {
            Signature = signature ?? throw new ArgumentNullException(nameof(signature));
            IsStatic = isStatic;
            Offset = offset;
            StaticOffset = staticOffset;
        }
    }
}
