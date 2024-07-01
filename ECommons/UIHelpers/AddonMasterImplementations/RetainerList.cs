using ECommons.Automation;
using ECommons.UIHelpers.AtkReaderImplementations;
using FFXIVClientStructs.FFXIV.Client.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster{
    public unsafe class RetainerList : AddonMasterBase<AddonRetainerList>
    {
        public RetainerList(nint addon) : base(addon)
        {
        }

        public RetainerList(void* addon) : base(addon)
        {
        }

        public bool Select(int retainerIndex)
        {
            if (retainerIndex < 0 || retainerIndex > 9) throw new ArgumentOutOfRangeException(nameof(retainerIndex));
            var reader = new ReaderRetainerList(Base);
            if (reader.Retainers.Count < retainerIndex && reader.Retainers[retainerIndex].IsActive)
            {
                Callback.Fire(Base, true, 2, (uint)retainerIndex, Callback.ZeroAtkValue, Callback.ZeroAtkValue);
                return true;
            }
            return false;
        }

        public bool Select(string retainerName)
        {
            var reader = new ReaderRetainerList(Base);
            for (int i = 0; i < reader.Retainers.Count; i++)
            {
                var r = reader.Retainers[i];
                if(r.IsActive && r.Name == retainerName)
                {
                    return Select(i);
                }
            }
            return false;
        }
    }
}
