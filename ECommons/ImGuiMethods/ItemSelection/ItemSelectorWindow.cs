using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.MathHelpers;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static ECommons.GenericHelpers;

namespace ECommons.ImGuiMethods.ItemSelection;
public class ItemSelectorWindow : Window
{
    private ICollection<uint> ItemCollection = null;
    private Func<uint> ItemSelected = null;
    private Predicate<uint> IsItemSelected = null;
    private WindowSystem WindowSystem = new();
    private List<Item> ItemList = [];
    private int PageNum = 0;

    /// <summary>
    /// Override or define this function to filter which items should be displayed for selection.
    /// </summary>
    public virtual Predicate<uint> ItemFilter { get; set; } = null;

    /// <summary>
    /// Creates item selector window to provide user with convenient item selection. 
    /// </summary>
    /// <param name="itemCollection">A collection where selected items will be stored. </param>
    /// <param name="windowSystem">If specified, window will be registered to the window system and opened, and unregistered and removed upon closing.</param>
    /// <param name="name">Title of the window.</param>
    public ItemSelectorWindow(ICollection<uint> itemCollection, WindowSystem? windowSystem = null, string name = "Select Items") : base(name, ImGuiWindowFlags.NoSavedSettings)
    {
        this.ItemCollection = itemCollection;
        Setup(windowSystem);
    }

    /// <summary>
    /// Creates item selector window to provide user with convenient item selection. 
    /// </summary>
    /// <param name="windowSystem">If specified, window will be registered to the window system and opened, and unregistered and removed upon closing.</param>
    /// <param name="name">Title of the window.</param>
    /// <param name="itemSelected">Function that will be triggered upon user selecting an item. Return true to close window, return false to keep it open.</param>
    /// <param name="isItemSelected">Whether item is selected or not. Can be null - no items will be highlighted as selected in that case. </param>
    public ItemSelectorWindow(Func<uint> itemSelected, Predicate<uint>? isItemSelected, WindowSystem? windowSystem = null, string name = "Select Items") : base(name, ImGuiWindowFlags.NoSavedSettings)
    {
        ItemSelected = itemSelected;
        IsItemSelected = isItemSelected;
        Setup(windowSystem);
    }

    public enum DisplayCategory
    {
        [Obfuscation] Weapons,
        [Obfuscation] Armor,
        [Obfuscation] Consumable,
        [Obfuscation] Materials,
        [Obfuscation] Housing,
        [Obfuscation] Crystals,
        [Obfuscation] Special,
        [Obfuscation] Other,
    }

    /// <summary>
    /// If you want, add, remove or completely override categories that are being displayed. Keep in mind that items that do not belong to any of these categories will not be displayed. If you want to get rid of category selector at all, clear it completely, then all items will be displayed. Set it to null to reset it to defaults. 
    /// </summary>
    public OrderedDictionary<string, uint[]> CategoryGroups
    {
        get
        {
            if(field == null)
            {
                field[DisplayCategory.Weapons.ToString()] = [.. ItemUICategory.Values.Where(x => x.OrderMajor == 1 || x.OrderMajor == 2 || x.RowId == 11).Select(x => x.RowId)];
                field[DisplayCategory.Armor.ToString()] = [.. ItemUICategory.Values.Where(x => x.RowId != 11 && x.RowId != 62).Where(x => x.OrderMajor == 3 || x.OrderMajor == 4).Select(x => x.RowId)];
                field[DisplayCategory.Consumable.ToString()] = [.. ItemUICategory.Values.Where(x => x.RowId.EqualsAny<uint>(44, 46, 33)).Select(x => x.RowId)];
                field[DisplayCategory.Materials.ToString()] = [.. ItemUICategory.Values.Where(x => x.OrderMajor == 6 && x.RowId < 90).Select(x => x.RowId)];
                field[DisplayCategory.Housing.ToString()] = [.. ItemUICategory.Values.Where(x => (x.OrderMajor == 6 && x.RowId > 90) || x.RowId.InRange(64, 80, true) || x.RowId.EqualsAny<uint>(95)).Select(x => x.RowId)];
                field[DisplayCategory.Crystals.ToString()] = [.. ItemUICategory.Values.Where(x => x.OrderMajor == 59).Select(x => x.RowId)];
                field[DisplayCategory.Special.ToString()] = [.. ItemUICategory.Values.Where(x => x.RowId.EqualsAny<uint>(100u, 39u, 112u)).Select(x => x.RowId)];
                var used = field.Values.SelectNested(x => x).ToHashSet();
                field[DisplayCategory.Other.ToString()] = [.. ItemUICategory.Values.Where(x => !used.Contains(x.RowId)).Select(x => x.RowId)];
            }
            return field;
        }
        set
        {
            field = value;
        }
    }

    public override void OnOpen()
    {
        RebuildItemList();
    }

    private void Setup(WindowSystem? windowSystem)
    {
        this.SizeConstraints = new()
        {
            MinimumSize = new(200, 100),
            MaximumSize = new(float.MaxValue),
        };
        if(!this.WindowName.Contains("##"))
        {
            this.WindowName += $"##{Guid.NewGuid()}";
        }
        WindowSystem = windowSystem;
        if(this.WindowSystem != null)
        {
            this.WindowSystem.AddWindow(this);
            this.IsOpen = true;
        }
    }

    private float TextLen
    {
        get
        {
            if(field == 0)
            {
                var sizes = this.ItemList.Select(x =>
                {
                    var name = x.GetName();
                    var size = ImGui.CalcTextSize(name).X;
                    size += ImGui.GetStyle().ItemSpacing.X * 4;
                    size += ImGui.GetFrameHeight() * 2;
                    return size;
                }).Order().ToArray();
                if(sizes.Length == 0)
                {
                    field = 0;
                }
                else
                {
                    var thold = sizes[(int)(sizes.Length * 0.8f)] * 1.5f;
                    field = MathF.Min(thold, sizes.Max());
                }
            }
            return field;
        }
        set => field = value;
    }

    public string SearchStr = "";
    public int MinILvl = 0;
    public int MaxILvl = (int)Item.Values.Max(x => x.LevelItem.RowId);
    public bool? Tradeable = null;
    public bool? Marketable = null;
    public bool? Desynth = null;
    public List<ItemRarity> Rarities = [];
    public List<uint> Categories = [];

    /// <summary>
    /// Redefine this selector window to work with a specified collection
    /// </summary>
    /// <param name="itemCollection"></param>
    public void Redefine(ICollection<uint> itemCollection)
    {
        this.ItemCollection = itemCollection;
        this.ItemSelected = null;
        this.IsItemSelected = null;
    }

    /// <summary>
    /// Redefine this selector window to work with specified functions
    /// </summary>
    public void Redefine(Func<uint> itemSelected, Predicate<uint>? isItemSelected = null)
    {
        this.ItemCollection = null;
        this.ItemSelected = itemSelected;
        this.IsItemSelected = isItemSelected;
    }

    /// <summary>
    /// Call this to draw in your own UI
    /// </summary>
    public sealed override void Draw()
    {
        ImGuiEx.SetNextItemFullWidth();
        ImGui.InputTextWithHint("##filter", "Search...", ref SearchStr);
        if(ImGui.IsItemDeactivated())
        {
            RebuildItemList();
        }
        var catcnt = 0;
        foreach(var catGroup in this.CategoryGroups)
        {
            if(catGroup.Value.Length == 0) continue;
            if(catcnt > 0)
            {
                var avail = ImGui.GetContentRegionAvail().X;
                var len = ImGui.GetFrameHeight() * 2 + ImGui.GetStyle().ItemSpacing.X * 3 + ImGui.CalcTextSize(catGroup.Key).X;
                if(len < avail)
                {
                    ImGui.SameLine();
                }
                else
                {
                    catcnt = -1;
                }
            }
            catcnt++;

            if(ThreadLoadImageHandler.TryGetIconTextureWrap(ItemUICategory.Get(catGroup.Value[0]).Icon, false, out var tex))
            {
                ImGui.Image(tex.Handle, new(ImGui.GetFrameHeight()));
                ImGui.SameLine();
            }
            if(ImGuiEx.CollectionCheckbox(catGroup.Key, catGroup.Value, Categories))
            {
                RebuildItemList();
            }
            if(catGroup.Value.Length > 1 && ImGuiEx.HoveredAndClicked("Right-click to select categories one by one", ImGuiMouseButton.Right))
            {
                ImGui.OpenPopup($"Cat{catGroup.Key}");
            }
            if(ImGui.BeginPopup($"Cat{catGroup.Key}"))
            {
                foreach(var x in catGroup.Value)
                {
                    var data = ItemUICategory.Get(x);
                    if(ThreadLoadImageHandler.TryGetIconTextureWrap(data.Icon, false, out var texv))
                    {
                        ImGui.Image(texv.Handle, new(ImGui.GetFrameHeight()));
                        ImGui.SameLine();
                    }
                    ImGuiEx.CollectionCheckbox($"{data.Name}", data.RowId, Categories);
                    ImGuiEx.DragDropRepopulate($"ISWCollect", data.RowId, Categories);
                }
                ImGui.EndPopup();
                if(!ImGui.IsPopupOpen($"Cat{catGroup.Key}"))
                {
                    RebuildItemList();
                }
            }
        }
        if(this.ItemList.Count == 0)
        {
            ImGuiEx.Text("No items matching your filters were found");
        }
        else
        {
            var perPage = 500;
            var numCol = 1;
            if(TextLen > 0)
            {
                numCol = (int)(ImGui.GetContentRegionAvail().X / TextLen) + 1;
            }
            var pages = ItemList.Count / perPage;
            if(pages > 1)
            {

            }
            if(ImGuiEx.BeginDefaultTable("ItemSelectTable", ["Item"], false, ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.NoSavedSettings | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInner, true))
            {
                if(PageNum < 0) PageNum = 0;
                var startNum = PageNum * perPage;
                if(startNum >= ItemList.Count)
                {
                    startNum = (ItemList.Count / 500) - 1;
                }
                for(int i = startNum; i < ItemList.Count; i++)
                {

                }
                ImGui.EndTable();
            }
        }
    }


    /// <summary>
    /// If you have changed any of search parameters, you must rebuild item list. 
    /// </summary>
    public void RebuildItemList()
    {
        this.ItemList.Clear();
        this.TextLen = 0f;
        foreach(var x in Item.Values)
        {
            if(x.GetName() == "") continue;
            if(this.Categories.Count > 0 && !this.Categories.Contains(x.ItemUICategory.RowId)) continue;
            if(this.SearchStr != "" && !ExcelItemHelper.GetName(x.RowId, true).Contains(this.SearchStr)) continue;
            if(!x.LevelItem.RowId.InRange((uint)this.MinILvl, (uint)this.MaxILvl, true)) continue;
            if(this.Tradeable != null && x.IsUntradable == Tradeable.Value) continue;
            if(this.Marketable != null && (x.ItemSearchCategory.RowId == 0) != this.Marketable.Value) continue;
            if(this.Desynth != null && (x.Desynth != 0) != this.Desynth) continue;
            if(this.Rarities.Count > 0 && !this.Rarities.Contains(x.GetRarity())) continue;
            this.ItemList.Add(x);
        }
    }

    public override void OnClose()
    {
        if(this.WindowSystem != null)
        {
            this.WindowSystem.RemoveWindow(this);
        }
    }

    private bool CalculatePagination(int maxItemsPerPage, out int startIndex, out int endIndex)
    {
        startIndex = endIndex = 0;
        if(ItemList.Count <= 0)
        {
            return false;
        }

        if(maxItemsPerPage <= 0)
            throw new ArgumentException("Max items per page must be greater than 0", nameof(maxItemsPerPage));

        if(ItemList.Count <= maxItemsPerPage)
        {
            
            return false;
        }

        int totalPages = (int)Math.Ceiling((double)ItemList.Count / maxItemsPerPage);
        int equalizedItemsPerPage = (int)Math.Ceiling((float)ItemList.Count / totalPages);

        int minPage = 0;
        int maxPage = totalPages - 1;
        this.PageNum = Math.Max(minPage, Math.Min(this.PageNum, maxPage));

        int pageIndex = this.PageNum;
        startIndex = pageIndex * equalizedItemsPerPage;
        endIndex = Math.Min(startIndex + equalizedItemsPerPage - 1, ItemList.Count - 1);

        return true; 
    }

    private struct PaginationResult
    {
        public int StartIndex;
        public int EndIndex;
        public int ActualPage;
        public int ItemsPerPage;
        public int TotalPages;
    }
}