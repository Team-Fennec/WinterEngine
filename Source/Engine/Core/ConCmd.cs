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

public abstract class ConCmd
{
    public abstract string Command { get; }
    public abstract string Description { get; }
    public abstract CmdFlags Flags { get; }

    public abstract void Exec(string[] args);
}
