using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.ImGuiMethods;

[AttributeUsage(AttributeTargets.All)]
public class EnumMemberNameAttribute(string name) : Attribute
{
    public string Name = name;
}
