using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Text.SeStringHandling;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ECommons.ChatMethods;
#nullable disable

public struct Sender : IEquatable<Sender>
{
    [Obfuscation] public string Name;
    [Obfuscation] public uint HomeWorld;

    public static bool TryParse(string nameWithWorld, out Sender s)
    {
        var split = nameWithWorld.Split('@');
        if(split.Length == 2)
        {
            var world = ExcelWorldHelper.Get(split[1]);
            if(world != null)
            {
                s = new(split[0], world.Value.RowId);
                return true;
            }
        }
        s = default;
        return false;
    }

    public Sender(string Name, uint HomeWorld)
    {
        this.Name = Name;
        this.HomeWorld = HomeWorld;
    }

    public Sender(SeString Name, uint HomeWorld)
    {
        this = new(Name.ToString(), HomeWorld);
    }

    public Sender(SeString Name, RowRef<World> HomeWorld)
    {
        this = new(Name.ToString(), HomeWorld.RowId);
    }

    public Sender(string Name, RowRef<World> HomeWorld)
    {
        this = new(Name, HomeWorld.RowId);
    }

    public Sender(IPlayerCharacter pc)
    {
        this = new(pc.Name, pc.HomeWorld);
    }

    public override bool Equals(object obj)
    {
        return obj is Sender sender && Equals(sender);
    }

    public bool Equals(Sender other)
    {
        return Name == other.Name &&
               HomeWorld == other.HomeWorld;
    }

    public IPlayerCharacter? Find()
    {
        foreach(var x in Svc.Objects)
        {
            if(x is IPlayerCharacter pc && pc.Name.ToString() == Name && pc.HomeWorld.RowId == HomeWorld) return pc;
        }
        return null;
    }

    public bool TryFind([NotNullWhen(true)] out IPlayerCharacter pc)
    {
        pc = Find();
        return pc != null;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, HomeWorld);
    }

    public override string ToString()
    {
        return $"{Name}@{Svc.Data.GetExcelSheet<World>()?.GetRowOrDefault(HomeWorld)?.Name}";
    }

    public static bool operator ==(Sender left, Sender right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Sender left, Sender right)
    {
        return !(left == right);
    }
}
