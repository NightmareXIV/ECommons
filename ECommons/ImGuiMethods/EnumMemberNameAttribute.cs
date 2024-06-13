using System;

namespace ECommons.ImGuiMethods;

[AttributeUsage(AttributeTargets.All)]
public class EnumMemberNameAttribute(string name) : Attribute
{
    public string Name = name;
}
