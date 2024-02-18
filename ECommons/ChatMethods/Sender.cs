﻿using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Text.SeStringHandling;
using ECommons.DalamudServices;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ECommons.ChatMethods;
#nullable disable

[Obfuscation(Exclude = true, ApplyToMembers = true)]
public struct Sender : IEquatable<Sender>
{
    public string Name;
    public uint HomeWorld;

    public Sender(string Name, uint HomeWorld)
    {
        this.Name = Name;
        this.HomeWorld = HomeWorld;
    }

    public Sender(SeString Name, uint HomeWorld)
    {
        this = new(Name.ToString(), HomeWorld);
    }

    public Sender(SeString Name, Dalamud.Game.ClientState.Resolvers.ExcelResolver<World> HomeWorld)
    {
        this = new(Name.ToString(), HomeWorld.Id);
    }

    public Sender(string Name, Dalamud.Game.ClientState.Resolvers.ExcelResolver<World> HomeWorld)
    {
        this = new(Name, HomeWorld.Id);
    }

    public Sender(PlayerCharacter pc)
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

    public PlayerCharacter? Find()
    {
        foreach (var x in Svc.Objects)
        {
            if (x is PlayerCharacter pc && pc.Name.ToString() == this.Name && pc.HomeWorld.Id == this.HomeWorld) return pc;
        }
        return null;
    }

    public bool TryFind([NotNullWhen(true)] out PlayerCharacter pc)
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
        return $"{this.Name}@{Svc.Data.GetExcelSheet<World>()?.GetRow(this.HomeWorld)?.Name}";
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
