using Dalamud.Interface.Utility;
using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using ECommons.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ConstrainedExecution;

namespace ECommons.ImGuiMethods;
public static unsafe partial class ImGuiEx
{
    public class RealtimeDragDrop 
    {
        public RealtimeDragDrop(string dragDropId)
        {
            this.DragDropID = dragDropId;
        }

        List<(Vector2 RowPos, Vector2 ButtonPos, Action BeginDraw, Action AcceptDraw)> MoveCommands = [];
        Vector2 InitialDragDropCurpos;
        Vector2 ButtonDragDropCurpos;
        string DragDropID;
        string? CurrentDrag = null;
        public void Begin()
        {
            MoveCommands.Clear();
        }

        public void NextRow()
        {
            InitialDragDropCurpos = ImGui.GetCursorPos();
        }

        public void DrawButtonDummy(string uniqueId, Action<string> onAcceptDragDropPayload)
        {
            ImGui.PushFont(UiBuilder.IconFont);
            ButtonDragDropCurpos = ImGui.GetCursorPos();
            var size = ImGuiHelpers.GetButtonSize(FontAwesomeIcon.ArrowsUpDownLeftRight.ToIconString());
            ImGui.Dummy(size);
            ImGui.PopFont();
            EndRow(uniqueId, onAcceptDragDropPayload);
        }

        public void SetRowColor(string uniqueId)
        {
            if(CurrentDrag == uniqueId)
            {
                var col = GradientColor.Get(EColor.Green, EColor.Green with { W = EColor.Green.W / 4 }, 500).ToUint();
                ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, col);
                ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg1, col);
            }
        }

        private void EndRow(string uniqueId, Action<string> onAcceptDragDropPayload)
        {
            MoveCommands.Add((InitialDragDropCurpos, ButtonDragDropCurpos, sourceAction, targetAction));

            void sourceAction()
            {
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.Button($"{FontAwesomeIcon.ArrowsUpDownLeftRight.ToIconString()}##{DragDropID}Move{uniqueId}");
                ImGui.PopFont();
                if(ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeAll);
                }
                if(ImGui.BeginDragDropSource(ImGuiDragDropFlags.SourceNoPreviewTooltip))
                {
                    ImGuiDragDrop.SetDragDropPayload(DragDropID, uniqueId);
                    CurrentDrag = uniqueId;
                    InternalLog.Verbose($"DragDropSource = {uniqueId}");
                    ImGui.EndDragDropSource();
                }
                else if(CurrentDrag == uniqueId)
                {
                    InternalLog.Verbose($"Current drag reset!");
                    CurrentDrag = null;
                }
            }

            void targetAction()
            {
                if(ImGui.BeginDragDropTarget())
                {
                    if(ImGuiDragDrop.AcceptDragDropPayload(DragDropID, out var payload, ImGuiDragDropFlags.AcceptBeforeDelivery | ImGuiDragDropFlags.AcceptNoDrawDefaultRect))
                    {
                        onAcceptDragDropPayload(payload);
                    }
                    ImGui.EndDragDropTarget();
                }
            }
        }

        public bool AcceptPayload([NotNullWhen(true)]out string? uniqueId, ImGuiDragDropFlags flags = ImGuiDragDropFlags.None)
        {
            uniqueId = null;
            if(ImGui.BeginDragDropTarget())
            {
                if(ImGuiDragDrop.AcceptDragDropPayload(this.DragDropID, out var payload, flags))
                {
                    uniqueId = payload;
                }
                ImGui.EndDragDropTarget();
            }
            return uniqueId != null;
        }

        public void End()
        {
            foreach(var x in MoveCommands)
            {
                ImGui.SetCursorPos(x.ButtonPos);
                x.BeginDraw();
                x.AcceptDraw();
                ImGui.SetCursorPos(x.RowPos);
                ImGui.Dummy(new Vector2(ImGui.GetContentRegionAvail().X, ImGuiHelpers.GetButtonSize(" ").Y));
                x.AcceptDraw();
            }
        }
    }
}
