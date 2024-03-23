using System;
using System.Collections.Generic;
using ValveKeyValue;

namespace WinterEngine.ToolsFramework;

public static class ToolsFramework
{
    private static readonly ILog log = LogManager.GetLogger("ToolsFramework");

    List<EngineTool> m_EngineTools = new List<EngineTool>();
    EngineTool m_CurrentTool;
    bool m_ToolsActive = true; // default to true

    public static EngineTool GetCurrentTool() => m_CurrentTool;
    public static IEnumerable<EngineTool> GetToolList() => m_EngineTools;

    public static void Init()
    {
        log.Info("Initializing Engine Tools...");

        // search for an enginetools.txt file around us
        if (!File.Exists("enginetools.vdf"))
        {
#if VERBOSE_LOGGING
            log.Info("No enginetools.txt found.");
#endif
            // none found
            goto skipautoload;
        }

        // run file through our kv interpreter
        var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
        KVObject engineToolsList = kv.Deserialize(File.OpenRead("enginetools.vdf"));

        // go through the entries and add the tools

        foreach (KVObject toolObj in (IEnumerable<KVObject>)engineToolsList.Value)
        {
            Engine.SendCommand($"tool_load {toolObj.Value.ToString()}");
        }

    skipautoload:
    }
}

internal sealed class LoadToolCommand : ConCmd<LoadToolCommand>
{
    public override string Command => "tool_load";
    public override string Description => "Loads a tool from the provided name";
    public override CmdFlags Flags => CmdFlags.None;

    public override void Exec(string[] args)
    {
        // todo: load tool command
    }
}
