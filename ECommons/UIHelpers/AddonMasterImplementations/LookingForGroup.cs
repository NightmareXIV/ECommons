using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe partial class AddonMaster
{
    public class LookingForGroup : AddonMasterBase
    {
        public LookingForGroup(nint addon) : base(addon)
        {
        }

        public LookingForGroup(void* addon) : base(addon)
        {
        }

        public bool RecruitMembersOrDetails() => ClickButtonIfEnabled(Base->GetButtonNodeById(46));
    }
}
