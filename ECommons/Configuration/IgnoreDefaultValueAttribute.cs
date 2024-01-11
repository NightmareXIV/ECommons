using System;

namespace ECommons.Configuration;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class IgnoreDefaultValueAttribute : Attribute
{
}  
