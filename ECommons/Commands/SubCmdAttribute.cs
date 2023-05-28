using System;

namespace ECommons.Commands;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class SubCmdAttribute : Attribute
{
    public string SubCommand { get; }
    public string HelpMessage { get; }

    public SubCmdAttribute(string subCommand, string helpMessage = "")
    {
        this.SubCommand = subCommand;
        this.HelpMessage = helpMessage;
    }
}