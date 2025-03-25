using ECommons.Automation;
using ECommons.ExcelServices;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Collections.Generic;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public unsafe partial class AddonMaster
{
    public class _CharaSelectListMenu : AddonMasterBase
    {
        public _CharaSelectListMenu(nint addon) : base(addon) { }
        public _CharaSelectListMenu(void* addon) : base(addon) { }

        public AtkComponentButton* WorldButton => Addon->GetButtonNodeById(4);
        public AtkComponentButton* NewCharacterButton => Addon->GetButtonNodeById(5);
        public AtkComponentButton* BackUpClientSettingsButton => Addon->GetButtonNodeById(6);

        public void World() => ClickButtonIfEnabled(WorldButton);
        public void NewCharacter() => ClickButtonIfEnabled(NewCharacterButton);
        public void BackUpClientSettings() => ClickButtonIfEnabled(BackUpClientSettingsButton);

        public bool TemporarilyLocked => false;// AgentLobby.Instance()->TemporaryLocked;

        public void SelectWorld()
        {
            var evt = CreateAtkEvent(1);
            var data = CreateAtkEventData().Build();
            Base->ReceiveEvent((AtkEventType)25, 1, &evt, &data);
        }

        public Character[] Characters
        {
            get
            {
                var ret = new List<Character>();
                var charaSpan = AgentLobby.Instance()->LobbyData.CharaSelectEntries.ToArray();
                for(var i = 0; i < charaSpan.Length; i++)
                {
                    var s = charaSpan[i];
                    ret.Add(new(this, i, s));
                }
                return [.. ret];
            }
        }

        public override string AddonDescription { get; } = "Character select menu";

        public class Character
        {
            private _CharaSelectListMenu Master;
            public int Index { get; init; }
            public CharaSelectCharacterEntry* Entry { get; init; }
            public string Name => Entry->NameString;
            public uint HomeWorld => Entry->HomeWorldId;
            public uint CurrentWorld => Entry->CurrentWorldId;
            public bool IsVisitingAnotherDC => Entry->LoginFlags.HasFlag(CharaSelectCharacterEntryLoginFlags.Unk32);
            public bool CanLoginNormally => !Entry->LoginFlags.HasFlag(CharaSelectCharacterEntryLoginFlags.Locked) && !Entry->LoginFlags.HasFlag(CharaSelectCharacterEntryLoginFlags.NameChangeRequired) && !Entry->LoginFlags.HasFlag(CharaSelectCharacterEntryLoginFlags.MissingExVersionForLogin) && !Entry->LoginFlags.HasFlag(CharaSelectCharacterEntryLoginFlags.Unk32);
            public bool IsSelected => AgentLobby.Instance()->HoveredCharacterContentId == Entry->ContentId;

            public Character(_CharaSelectListMenu master, int index, CharaSelectCharacterEntry* entry)
            {
                Master = master;
                Index = index;
                Entry = entry;
            }

            public void Select()
            {
                Callback.Fire(Master.Base, true, 21, Index);
            }

            public void Login() => Click(false);

            public void OpenContextMenu() => Click(true);

            private void Click(bool right)
            {
                var eventIndex = (byte)(5 + Index);
                var evt = stackalloc AtkEvent[] { Master.CreateAtkEvent(eventIndex) };
                var data = stackalloc AtkEventData[] { Master.CreateAtkEventData().Write<byte>(6, (byte)(right ? 1 : 0)).Build() };
                Master.Base->ReceiveEvent(AtkEventType.MouseClick, eventIndex, evt, data);
            }

            public override string? ToString()
            {
                return $"{Name}@{ExcelWorldHelper.GetName(HomeWorld)}";
            }
        }
    }
}
