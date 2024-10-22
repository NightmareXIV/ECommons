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
public unsafe partial class AddonMaster
{
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
                for(var i = 0; i < 16; i++)
                {
                    var str = stringArray->StringArray[i];
                    var worldName = MemoryHelper.ReadStringNullTerminated((nint)str).Trim();
                    if(worldName.IsNullOrEmpty()) break;
                    ret.Add(new(this, i, worldName));
                }
                return [.. ret];
            }
        }

        public override string AddonDescription { get; } = "World selection menu on login screen";

        public class World
        {
            public readonly _CharaSelectWorldServer Master;
            public readonly int Index;
            public readonly string Name;

            public World(_CharaSelectWorldServer master, int index, string name)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(index);
                Index = index;
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Master = master;
            }

            public void Select()
            {
                /*var evt = Master.CreateAtkEvent();
                var data = Master.CreateAtkEventData()
                    .Write<byte>(0x10, (byte)Index)
                    .Write<byte>(0x16, (byte)Index)
                    .Build();
                Master.Base->ReceiveEvent((AtkEventType)35, 0, &evt, &data);
                Master.Base->ReceiveEvent((AtkEventType)37, 0, &evt, &data);*/
                Callback.Fire(Master.Base, true, 25, 0, Index);
            }
        }
    }
}