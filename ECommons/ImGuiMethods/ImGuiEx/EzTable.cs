using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommons.ImGuiMethods;
public static unsafe partial class ImGuiEx
{
    private static int a;
    public static void EzTable(IEnumerable<EzTableEntry> entries) => EzTable(null, null, entries, true);
    public static void EzTable(string? id, IEnumerable<EzTableEntry> entries) => EzTable(id, null, entries, true);
    public static void EzTable(ImGuiTableFlags? tableFlags, IEnumerable<EzTableEntry> entries) => EzTable(null, tableFlags, entries, true);
    public static void EzTable(string? ID, ImGuiTableFlags? tableFlags, IEnumerable<EzTableEntry> entries, bool header)
    {
        if(!entries.Any())
        {
            ImGuiEx.Text(EzColor.RedBright, $"Table contains no elements!");
            return;
        }
        var entriesArray = entries.ToArray();
        ID ??= GenericHelpers.GetCallStackID();
        var flags = tableFlags ?? (ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingFixedFit);
        var size = entriesArray.Length / entriesArray.Count(x => x.ColumnName == entriesArray.First().ColumnName);
        if(ImGui.BeginTable(ID, size, flags))
        {
            for(var i = 0; i < size; i++)
            {
                ImGui.TableSetupColumn(entriesArray[i].ColumnName, entriesArray[i].ColumnFlags);
            }
            if(header) ImGui.TableHeadersRow();
            for(var i = 0; i < entriesArray.Length; i++)
            {
                if(i % size == 0) ImGui.TableNextRow();
                ImGui.TableNextColumn();
                try
                {
                    entriesArray[i].Delegate();
                }
                catch(Exception e)
                {
                    e.Log();
                }
            }
            ImGui.EndTable();
        }
    }

    public struct EzTableEntry
    {
        public string ColumnName;
        public Action Delegate;
        public ImGuiTableColumnFlags ColumnFlags;

        public EzTableEntry(string columnName, ImGuiTableColumnFlags? columnFlags, Action @delegate)
        {
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
            Delegate = @delegate ?? throw new ArgumentNullException(nameof(@delegate));
            ColumnFlags = columnFlags ?? ImGuiTableColumnFlags.None;
        }

        public EzTableEntry(string columnName, Action @delegate)
        {
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
            Delegate = @delegate ?? throw new ArgumentNullException(nameof(@delegate));
            ColumnFlags = ImGuiTableColumnFlags.None;
        }
        public EzTableEntry(string columnName, bool stretch, Action @delegate)
        {
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
            Delegate = @delegate ?? throw new ArgumentNullException(nameof(@delegate));
            ColumnFlags = stretch ? ImGuiTableColumnFlags.WidthStretch : ImGuiTableColumnFlags.WidthFixed;
        }
    }
}