using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class SelectString : AddonMasterBase<AddonSelectString>
    {
        public SelectString(nint addon) : base(addon)
        {
        }

        public SelectString(void* addon) : base(addon) { }

        public Entry[] Entries
        {
            get
            {
                var ret = new Entry[this.Addon->PopupMenu.PopupMenu.EntryCount];
                for (int i = 0; i < ret.Length; i++)
                {
                    ret[i] = new(this.Addon, i);
                }
                return ret;
            }
        }

        public struct Entry
        {
            private AddonSelectString* Addon;
            public int Index { get; init; }

            public Entry(AddonSelectString* addon, int index)
            {
                Addon = addon;
                Index = index;
            }

            public SeString SeString => MemoryHelper.ReadSeStringNullTerminated((nint)this.Addon->PopupMenu.PopupMenu.EntryNames[Index]);
            public string Text => SeString.ExtractText();

            public readonly void Select()
            {
                Callback.Fire((AtkUnitBase*)this.Addon, true, Index);
            }

            public override string? ToString()
            {
                return $"AddonMaster.SelectString.Entry [Text=\"{Text}\", Index={Index}]";
            }
        }
    }
}

[Obsolete("Please use AddonMaster.SelectString")]
public unsafe class SelectStringMaster : AddonMaster.SelectString
{
    public SelectStringMaster(nint addon) : base(addon)
    {
    }

    public SelectStringMaster(void* addon) : base(addon)
    {
    }
}