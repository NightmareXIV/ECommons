using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Memory;
using ECommons.Automation;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class SelectIconString : AddonMasterBase<AddonSelectIconString>
    {
        public SelectIconString(nint addon) : base(addon)
        {
        }

        public SelectIconString(void* addon) : base(addon) { }

        public Entry[] Entries
        {
            get
            {
                var ret = new Entry[Addon->PopupMenu.PopupMenu.EntryCount];
                for (var i = 0; i < ret.Length; i++)
                    ret[i] = new(Addon, i);
                return ret;
            }
        }

        public struct Entry(AddonSelectIconString* addon, int index)
        {
            private AddonSelectIconString* Addon = addon;
            public int Index { get; init; } = index;

            public SeString SeString => MemoryHelper.ReadSeStringNullTerminated((nint)Addon->PopupMenu.PopupMenu.EntryNames[Index]);
            public string Text => SeString.ExtractText();

            public readonly void Select()
            {
                Callback.Fire((AtkUnitBase*)Addon, true, Index);
            }

            public override string? ToString()
            {
                return $"AddonMaster.SelectString.Entry [Text=\"{Text}\", Index={Index}]";
            }
        }
    }
}
