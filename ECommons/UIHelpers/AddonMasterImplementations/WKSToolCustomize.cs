using Dalamud.Memory;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;

namespace ECommons.UIHelpers.AddonMasterImplementations;

public partial class AddonMaster
{
    /// <summary>
    /// Moon Recipe Notebook
    /// Works similary to the normal recipe, except it's specialized to moon items you're doing the quest for
    /// </summary>
    public unsafe partial class WKSToolCustomize : AddonMasterBase<AtkUnitBase>
    {
        public WKSToolCustomize(nint addon) : base(addon) { }
        public WKSToolCustomize(void* addon) : base(addon) { }

        public ClassSelector[] ClassList
        {
            get
            {
                var ret = new List<ClassSelector>();
                for (var i = 0; i < 11; i++)
                {
                    var level = Addon->AtkValues[22 + i].UInt;
                    if (level == 0)
                        continue;

                    var ClassName = MemoryHelper.ReadSeStringNullTerminated((nint)Addon->AtkValues[11 + i].String.Value).GetText();
                    var ClassList = new ClassSelector(this, i)
                    {
                        ClassName = ClassName,
                        Level = level,
                    };
                    ret.Add(ClassList);
                }
                return [.. ret];
            }
        }

        public class ClassSelector(WKSToolCustomize master, int index)
        {
            public required string ClassName;
            public uint Level;

            public void Select()
            {
                Callback.Fire(master.Base, true, 11, index);
            }
        }

        public override string AddonDescription => "Cosmic Relic Tool Ui";
    }
}
