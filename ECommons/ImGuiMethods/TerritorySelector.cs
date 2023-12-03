using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ECommons.SimpleGui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.ImGuiMethods
{
    public unsafe class TerritorySelector : Window
    {
        WindowSystem WindowSystem;
        HashSet<uint> SelectedTerritories;
        Action<HashSet<uint>> Callback;

        public TerritorySelector(Action<HashSet<uint>> Callback, string TitleName = "Select zones") : base(TitleName)
        {
            Setup([], Callback);
        }

        public TerritorySelector(IEnumerable<uint> SelectedTerritories, Action<HashSet<uint>> Callback, string TitleName = "Select zones") : base(TitleName)
        {
            Setup(SelectedTerritories?.ToHashSet() ?? [], Callback);
        }

        void Setup(HashSet<uint> SelectedTerritories, Action<HashSet<uint>> Callback)
        {
            this.SelectedTerritories = SelectedTerritories;
            this.Callback = Callback;
            WindowSystem = new($"ECommonsTerritorySelector_{Guid.NewGuid()}");
            WindowSystem.AddWindow(this);
            Svc.PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
        }

        public override void Draw()
        {
            
        }

        public override void OnClose()
        {
            Svc.PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        }

        [Obsolete("Do not reopen existing TerritorySelector. You should create new TerritorySelector every time.", true)]
        new bool IsOpen => throw new Exception("You should create new TerritorySelector every time.");

        public void Close()
        {
            base.IsOpen = false;
        }
    }
}
