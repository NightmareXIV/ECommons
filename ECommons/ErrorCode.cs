using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommons;

/// <summary>
/// An enum that describes common errors that happen primarily for IPC communication use. These should not contain specific plugin names.
/// </summary>
public enum ErrorCode
{
    [Obfuscation] Success,
    [Obfuscation] Player_is_not_logged_in,
    [Obfuscation] Invalid_world_specified,
    [Obfuscation] Plugin_is_busy,
}