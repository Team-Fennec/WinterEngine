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

#if HAS_MACROS
//#macro ConsoleCommand(name, desc, fn) internal sealed class nameConCommand : ConCmd {\
    public override string Command => "name";\
    public override string Description => desc;\
    public override CmdFlags Flags => CmdFlags.None;\
    public override void Exec(string[] args)\
    fn\
}
#endif

public abstract class ConCmd
{
    public abstract string Command { get; }
    public abstract string Description { get; }
    public abstract CmdFlags Flags { get; }

    public abstract void Exec(string[] args);
}
