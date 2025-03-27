using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using ECommons.Schedulers;
using ImGuiNET;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ECommons.ImGuiMethods.TerritorySelection;
#nullable disable

public unsafe class TerritorySelector : Window
{
    public enum Category { World, Housing, Inn, Dungeon, Raid, Trial, Deep_Dungeon, Other, All }
    public enum Column { ID, Zone, Region, IntendedUse }
    public enum DisplayMode { PlaceNameDutyUnion, PlaceNameAndDuty, PlaceNameOnly }

    public static bool Singleton = true;
    public static Dictionary<Category, TerritoryIntendedUseEnum[]> Categories = new()
    {
        [Category.World] = [TerritoryIntendedUseEnum.City_Area, TerritoryIntendedUseEnum.Open_World,],
        [Category.Housing] = [TerritoryIntendedUseEnum.Housing_Instances, TerritoryIntendedUseEnum.Residential_Area,],
        [Category.Inn] = [TerritoryIntendedUseEnum.Inn,],
        [Category.Dungeon] = [TerritoryIntendedUseEnum.Dungeon, TerritoryIntendedUseEnum.Variant_Dungeon, TerritoryIntendedUseEnum.Criterion_Duty, TerritoryIntendedUseEnum.Criterion_Savage_Duty,],
        [Category.Raid] = [TerritoryIntendedUseEnum.Raid, TerritoryIntendedUseEnum.Raid_2, TerritoryIntendedUseEnum.Alliance_Raid, TerritoryIntendedUseEnum.Large_Scale_Raid, TerritoryIntendedUseEnum.Large_Scale_Savage_Raid,],
        [Category.Trial] = [TerritoryIntendedUseEnum.Trial],
        [Category.Deep_Dungeon] = [TerritoryIntendedUseEnum.Deep_Dungeon],
    };
    public static readonly List<TerritorySelector> Selectors = [];
    private readonly Dictionary<Category, List<TerritoryType>> Cache = [];

    public bool OnlySelected = false;
    public string Filter = "";
    public uint[] HiddenTerritories = [];
    public Category[] HiddenCategories = [];
    public Category? SelectedCategory = null;
    private WindowSystem WindowSystem;
    private HashSet<uint> SelectedTerritories;
    private uint SelectedTerritory;
    private bool IsSingleSelection = false;
    private Action<TerritorySelector, HashSet<uint>> Callback;
    private Action<TerritorySelector, uint> CallbackSingle;
    private static bool? VisibleAction = null;
    public Column[] ExtraColumns = [Column.ID, Column.Zone, Column.Region, Column.IntendedUse];
    public DisplayMode Mode = DisplayMode.PlaceNameAndDuty;

    public Action<TerritoryType, Vector4?, string> ActionDrawPlaceName = (TerritoryType t, Vector4? col, string name) =>
    {
        ImGuiEx.Text(col, name);
    };

    public TerritorySelector(uint SelectedTerritory, Action<TerritorySelector, uint> Callback, string TitleName = null) : base(TitleName)
    {
        new TickScheduler(() => Setup([SelectedTerritory], null, Callback));
    }

    public TerritorySelector(Action<TerritorySelector, uint> Callback, string TitleName = null) : base(TitleName)
    {
        new TickScheduler(() => Setup([], null, Callback));
    }

    public TerritorySelector(IEnumerable<uint> SelectedTerritories, Action<TerritorySelector, HashSet<uint>> Callback, string TitleName = null) : base(TitleName)
    {
        new TickScheduler(() => Setup(SelectedTerritories?.ToHashSet() ?? [], Callback, null));
    }

    public TerritorySelector(Action<TerritorySelector, HashSet<uint>> Callback, string TitleName = null) : base(TitleName)
    {
        new TickScheduler(() => Setup([], Callback, null));
    }

    private void Setup(HashSet<uint> SelectedTerritories, Action<TerritorySelector, HashSet<uint>> Callback, Action<TerritorySelector, uint> CallbackSingle)
    {
        WindowName ??= "Select zones";
        IsSingleSelection = CallbackSingle != null;
        SelectedTerritory = SelectedTerritories.FirstOrDefault();
        if(Singleton)
        {
            Selectors.Each(x => x.Close());
            Selectors.Clear();
        }
        else
        {
            if(Selectors.Any(x => x.WindowName == WindowName))
            {
                Notify.Error("Territory selector is already open");
                return;
            }
        }
        Selectors.Add(this);
        this.SelectedTerritories = SelectedTerritories;
        this.Callback = Callback;
        this.CallbackSingle = CallbackSingle;
        WindowSystem = new($"ECommonsTerritorySelector_{Guid.NewGuid()}");
        WindowSystem.AddWindow(this);
        Svc.PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
        base.IsOpen = true;
        //yes I know it's not optimized but it's one-time call so whatever okay?
        foreach(var c in Categories.Where(x => !HiddenCategories.Contains(x.Key)))
        {
            foreach(var x in Svc.Data.GetExcelSheet<TerritoryType>().Where(x => !HiddenTerritories.Contains(x.RowId)))
            {
                if(c.Value.Contains((TerritoryIntendedUseEnum)x.TerritoryIntendedUse.RowId) && x.PlaceName.ValueNullable?.Name.GetText().IsNullOrEmpty() == false)
                {
                    if(!Cache.TryGetValue(c.Key, out var value))
                    {
                        value = ([]);
                        Cache[c.Key] = value;
                    }

                    value.Add(x);
                }
            }
        }
        if(!HiddenCategories.Contains(Category.Other))
        {
            Cache[Category.Other] = [];
            foreach(var x in Svc.Data.GetExcelSheet<TerritoryType>().Where(x => !HiddenTerritories.Contains(x.RowId)))
            {
                if(!Cache.Values.Any(c => c.Any(z => z.RowId == x.RowId)) && x.PlaceName.ValueNullable?.Name.GetText().IsNullOrEmpty() == false)
                {
                    Cache[Category.Other].Add(x);
                }
            }
        }
        if(!HiddenCategories.Contains(Category.All))
        {
            Cache[Category.All] = [];
            foreach(var x in Svc.Data.GetExcelSheet<TerritoryType>().Where(x => !HiddenTerritories.Contains(x.RowId)))
            {
                if(x.PlaceName.ValueNullable?.Name.GetText().IsNullOrEmpty() == false)
                {
                    Cache[Category.All].Add(x);
                }
            }
        }
        foreach(var x in Cache.ToArray())
        {
            if(x.Value.Count == 0)
            {
                Cache.Remove(x);
            }
        }
    }

    public override void Draw()
    {
        VisibleAction = null;
        if(ImGui.BeginTabBar("##TerritorySelectorBar"))
        {
            foreach(var x in Cache)
            {
                if(ImGuiEx.BeginTabItem(x.Key.ToString(), SelectedCategory == x.Key ? ImGuiTabItemFlags.SetSelected : ImGuiTabItemFlags.None))
                {
                    ImGui.SetNextItemWidth(200f);
                    ImGui.InputTextWithHint($"##search", "Filter...", ref Filter, 50);
                    ImGui.SameLine();
                    ImGui.Checkbox("Only selected", ref OnlySelected);
                    if(Player.Available)
                    {
                        ImGui.SameLine();
                        if(!IsSingleSelection)
                        {
                            if(ImGuiEx.CollectionCheckbox($"Current: {ExcelTerritoryHelper.GetName(Svc.ClientState.TerritoryType)}", Svc.ClientState.TerritoryType, SelectedTerritories))
                            {
                                try
                                {
                                    Callback(this, SelectedTerritories);
                                }
                                catch(Exception e)
                                {
                                    e.Log();
                                }
                            }
                            ImGui.SameLine();
                            if(ImGui.Button("Add all visible"))
                            {
                                VisibleAction = true;
                            }
                            ImGui.SameLine();
                            if(ImGui.Button("Remove all visible"))
                            {
                                VisibleAction = false;
                            }
                        }
                        else
                        {
                            if(ImGui.RadioButton($"Current zone: {ExcelTerritoryHelper.GetName(Svc.ClientState.TerritoryType)}", SelectedTerritory == Svc.ClientState.TerritoryType))
                            {
                                SelectedTerritory = Svc.ClientState.TerritoryType;
                                try
                                {
                                    CallbackSingle(this, SelectedTerritory);
                                }
                                catch(Exception e)
                                {
                                    e.Log();
                                }
                            }
                        }
                    }

                    if(ImGui.BeginChild("##ChildTable"))
                    {
                        if(ImGui.BeginTable("##TSelector", 2 + ExtraColumns.Length + (Mode == DisplayMode.PlaceNameAndDuty ? 1 : 0), ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.NoSavedSettings | ImGuiTableFlags.SizingFixedFit))
                        {
                            ImGui.TableSetupColumn(" ");
                            if(ExtraColumns.Contains(Column.ID)) ImGui.TableSetupColumn("ID");
                            if(Mode == DisplayMode.PlaceNameDutyUnion)
                            {
                                ImGui.TableSetupColumn("Place Name/Duty", ImGuiTableColumnFlags.WidthStretch);
                            }
                            else if(Mode == DisplayMode.PlaceNameOnly)
                            {
                                ImGui.TableSetupColumn("Place Name", ImGuiTableColumnFlags.WidthStretch);
                            }
                            else
                            {
                                ImGui.TableSetupColumn("Place Name", ImGuiTableColumnFlags.WidthStretch);
                                ImGui.TableSetupColumn("Duty");
                            }
                            if(ExtraColumns.Contains(Column.Zone)) ImGui.TableSetupColumn("Zone");
                            if(ExtraColumns.Contains(Column.Region)) ImGui.TableSetupColumn("Region");
                            if(ExtraColumns.Contains(Column.IntendedUse)) ImGui.TableSetupColumn("Intended use");

                            ImGui.TableHeadersRow();

                            foreach(var t in x.Value)
                            {
                                var cfc = t.ContentFinderCondition.ValueNullable?.Name.GetText() ?? "";
                                var questBattle = t.QuestBattle.ValueNullable?.Quest.GetValueOrDefault<Quest>()?.Name.GetText() ?? "";
                                var name = t.PlaceName.ValueNullable?.Name.GetText() ?? "";
                                var zone = t.PlaceNameZone.ValueNullable?.Name.GetText() ?? "";
                                var region = t.PlaceNameRegion.ValueNullable?.Name.GetText() ?? "";
                                var intended = ((TerritoryIntendedUseEnum)t.TerritoryIntendedUse.RowId).ToString().Replace("_", " ") ?? "";
                                var col = t.RowId == Svc.ClientState.TerritoryType && Player.Available;

                                if(Filter != ""
                                    && !cfc.Contains(Filter, StringComparison.OrdinalIgnoreCase)
                                    && !questBattle.Contains(Filter, StringComparison.OrdinalIgnoreCase)
                                    && !name.Contains(Filter, StringComparison.OrdinalIgnoreCase)
                                    && !zone.Contains(Filter, StringComparison.OrdinalIgnoreCase)
                                    && !region.Contains(Filter, StringComparison.OrdinalIgnoreCase)
                                    && !intended.Contains(Filter, StringComparison.OrdinalIgnoreCase)
                                    && !x.Key.ToString().Contains(Filter, StringComparison.OrdinalIgnoreCase)
                                    && !t.RowId.ToString().Contains(Filter, StringComparison.OrdinalIgnoreCase))
                                {
                                    continue;
                                }

                                if(OnlySelected && !SelectedTerritories.Contains(t.RowId))
                                {
                                    continue;
                                }

                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();//checkbox
                                if(!IsSingleSelection)
                                {
                                    if(ImGuiEx.CollectionCheckbox($"##sel{t.RowId}", t.RowId, SelectedTerritories))
                                    {
                                        try
                                        {
                                            Callback(this, SelectedTerritories);
                                        }
                                        catch(Exception e)
                                        {
                                            e.Log();
                                        }
                                    }
                                    if(VisibleAction == true && !SelectedTerritories.Contains(t.RowId))
                                    {
                                        SelectedTerritories.Add(t.RowId);
                                        Callback(this, SelectedTerritories);
                                    }
                                    if(VisibleAction == false && SelectedTerritories.Contains(t.RowId))
                                    {
                                        SelectedTerritories.Remove(t.RowId);
                                        Callback(this, SelectedTerritories);
                                    }
                                }
                                else
                                {
                                    if(ImGui.RadioButton($"##sel{t.RowId}", t.RowId == SelectedTerritory))
                                    {
                                        SelectedTerritory = t.RowId;
                                        try
                                        {
                                            CallbackSingle(this, SelectedTerritory);
                                        }
                                        catch(Exception e)
                                        {
                                            e.Log();
                                        }
                                    }
                                }
                                if(ExtraColumns.Contains(Column.ID))
                                {
                                    ImGui.TableNextColumn(); //id
                                    ImGuiEx.Text($"{t.RowId}");
                                }

                                if(Mode == DisplayMode.PlaceNameDutyUnion)
                                {
                                    ImGui.TableNextColumn();
                                    ImGuiEx.Text(cfc ?? questBattle ?? zone);
                                }
                                else if(Mode == DisplayMode.PlaceNameOnly)
                                {
                                    ImGui.TableNextColumn();
                                    ImGuiEx.Text(zone);
                                }
                                else
                                {

                                    ImGui.TableNextColumn(); //Place name
                                    ActionDrawPlaceName(t, col ? ImGuiColors.DalamudOrange : ImGuiColors.DalamudYellow, name);

                                    ImGui.TableNextColumn(); //Duty
                                    if(!cfc.IsNullOrEmpty())
                                    {
                                        ImGuiEx.Text($"{cfc}");
                                    }
                                    else if(!questBattle.IsNullOrEmpty())
                                    {
                                        ImGuiEx.Text($"{questBattle}");
                                    }
                                    else
                                    {
                                        ImGuiEx.Text("");
                                    }
                                }

                                if(ExtraColumns.Contains(Column.Zone))
                                {
                                    ImGui.TableNextColumn(); //zone
                                    ImGuiEx.Text($"{zone}");
                                }

                                if(ExtraColumns.Contains(Column.Region))
                                {
                                    ImGui.TableNextColumn(); //Region
                                    ImGuiEx.Text($"{region}");
                                }

                                if(ExtraColumns.Contains(Column.IntendedUse))
                                {
                                    ImGui.TableNextColumn(); //use
                                    ImGuiEx.Text($"{intended}");
                                }
                            }


                            ImGui.EndTable();
                        }
                    }
                    ImGui.EndChild();
                    ImGui.EndTabItem();
                }
            }
            ImGui.EndTabBar();
            SelectedCategory = null;
        }
    }

    public override void OnClose()
    {
        Svc.PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        Selectors.Remove(this);
    }

    [Obsolete("Do not reopen existing TerritorySelector. You should create new TerritorySelector every time.", true)]
    private new bool IsOpen => throw new Exception("You should create new TerritorySelector every time.");

    public void Close()
    {
        base.IsOpen = false;
    }
}
