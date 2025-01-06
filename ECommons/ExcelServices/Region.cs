using System;
using System.Reflection;

namespace ECommons.ExcelServices;

[Obfuscation(Exclude = true, ApplyToMembers = true)]
[Obsolete($"Use {nameof(ExcelWorldHelper.Region)}", true)]
public enum Region
{
    JP = 1, NA = 2, EU = 3, OC = 4
}
