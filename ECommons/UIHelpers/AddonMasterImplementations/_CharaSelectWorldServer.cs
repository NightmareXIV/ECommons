using Dalamud.Memory;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe partial class AddonMaster{
    public class _CharaSelectWorldServer : AddonMasterBase
    {
        public _CharaSelectWorldServer(nint addon) : base(addon)
        {
        }

        public _CharaSelectWorldServer(void* addon) : base(addon)
        {
        }

        public World[] Worlds
        {
            get
            {
                var ret = new List<World>();
                var stringArray = RaptureAtkModule.Instance()->AtkArrayDataHolder.StringArrays[1];
                for (int i = 0; i < 16; i++)
                {
                    var str = stringArray->StringArray[i];
                    var worldName = MemoryHelper.ReadStringNullTerminated((nint)str).Trim();
                    if (worldName.IsNullOrEmpty()) break;
                    ret.Add(new(i, worldName, Base));
                }
                return [.. ret];
            }
        }

        public class World
        {
            public readonly int Index;
            public readonly string Name;
            public readonly AtkUnitBase* Base;

            public World(int index, string name, AtkUnitBase* @base)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(index);
                Index = index;
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Base = @base;
            }

            public void Select()
            {
                Callback.Fire(Base, true, 25, 0, Index);
            }
        }
    }
}