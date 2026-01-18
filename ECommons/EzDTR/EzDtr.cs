using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using ECommons.DalamudServices;
using ECommons.Reflection;
using System;
using System.Collections.Generic;

namespace ECommons.EzDTR;

/// <summary>
/// Provides wrapped access to an IDtrBarEntry. Creates an update event to update the text and click event. Disposed automatically upon calling <see cref="ECommonsMain.Dispose"/>.
/// </summary>
public class EzDtr : IDisposable
{
    public IDtrBarEntry? Entry;
    internal static List<EzDtr> Registered = [];
    internal Func<SeString> Text;
    internal Action<DtrInteractionEvent>? OnClick;
    internal Func<bool>? ShowCondition;

    /// <summary>
    /// Creates a new <see cref="EzDtr"/>
    /// </summary>
    /// <param name="text">Function that returns an <see cref="SeString"/> for the entry's text.</param>
    /// <param name="onClick">Action performed whenever the entry is clicked</param>
    /// <param name="title">Name of the Dtr entry. Defaults to the plugin name.</param>
    public EzDtr(Func<SeString> text, Action? onClick = null, string? title = null, Func<bool> showCondition = null)
    {
        title ??= DalamudReflector.GetPluginName();
        Text = text;
        OnClick = onClick != null ? _ => onClick() : null;
        Entry ??= Svc.DtrBar.Get(title);
        ShowCondition = showCondition;
        Svc.Framework.Update += OnUpdate;
        Registered.Add(this);
    }

    public EzDtr(Func<SeString> text, Action<DtrInteractionEvent>? onClick = null, string? title = null, Func<bool> showCondition = null)
    {
        title ??= DalamudReflector.GetPluginName();
        Text = text;
        OnClick = onClick;
        Entry ??= Svc.DtrBar.Get(title);
        ShowCondition = showCondition;
        Svc.Framework.Update += OnUpdate;
        Registered.Add(this);
    }

    internal void OnUpdate(object _)
    {
        if(Entry != null)
        {
            if(ShowCondition != null && !ShowCondition())
            {
                Entry.Shown = false;
                return;
            }
            Entry.Shown = true;
            Entry.Text = Text();
            if(OnClick != null)
                Entry.OnClick = OnClick;
        }
    }

    public void Dispose()
    {
        Svc.Framework.Update -= OnUpdate;
        Registered.Remove(this);
        Entry?.Remove();
    }

    public static void DisposeAll() => Registered.ToArray().Each(x => x.Dispose());
}
