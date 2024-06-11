using System;
using System.Diagnostics;

namespace WinterEngine.Core;

[Flags]
public enum CmdFlags
{
    None,
    Cheat,
    Development,
    Debug
}

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class ConCmdAttribute : Attribute
{
    public string Command;
    public string Description;
    public CmdFlags Flags;

    public ConCmdAttribute(string command, string description, CmdFlags flags)
    {
        Command = command;
        Description = description;
        Flags = flags;
    }
}
