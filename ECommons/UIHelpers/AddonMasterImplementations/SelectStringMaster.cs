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
public unsafe class SelectStringMaster : AddonMasterBase<AddonSelectString>
{
    public SelectStringMaster(nint addon) : base(addon)
    {
    }

    public SelectStringMaster(void* addon) : base(addon) { }

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
        private int Index;

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
    }
}
