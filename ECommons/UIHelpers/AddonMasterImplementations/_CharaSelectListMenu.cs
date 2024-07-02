using ECommons.Automation;
using ECommons.ExcelServices;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe partial class AddonMaster{
    public class _CharaSelectListMenu : AddonMasterBase
    {
        public _CharaSelectListMenu(nint addon) : base(addon)
        {
        }

        public _CharaSelectListMenu(void* addon) : base(addon)
        {
        }

        public bool TemporarilyLocked => AgentLobby.Instance()->TemporaryLocked;

        public Character[] Characters
        {
            get
            {
                var ret = new List<Character>();
                var charaSpan = AgentLobby.Instance()->LobbyData.CharaSelectEntries.ToArray();
                for (var i = 0; i < charaSpan.Length; i++)
                {
                    var s = charaSpan[i];
                    ret.Add(new(Base, i, s));
                }
                return [.. ret];
            }
        }

        public class Character
        {
            public AtkUnitBase* Base { get; init; }
            public int Index { get; init; }
            public CharaSelectCharacterEntry* Entry { get; init; }
            public string Name => Entry->NameString;
            public uint HomeWorld => Entry->HomeWorldId;

            public Character(AtkUnitBase* @base, int index, CharaSelectCharacterEntry* entry)
            {
                Base = @base;
                Index = index;
                Entry = entry;
            }

            public void Select()
            {
                Callback.Fire(Base, true, 21, Index);
            }

            public void Login()
            {
                Callback.Fire(Base, true, 29, 0, Index);
            }

            public void OpenContextMenu()
            {
                Callback.Fire(Base, true, 29, 1, Index);
            }

            public override string? ToString()
            {
                return $"{Name}@{ExcelWorldHelper.GetName(HomeWorld)}";
            }
        }
    }
}
