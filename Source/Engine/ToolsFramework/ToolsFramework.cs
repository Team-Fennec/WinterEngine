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

    internal static object GetCurrentTool()
    {
        throw new NotImplementedException();
    }

    internal static IEnumerable<EngineTool> GetToolList()
    {
        throw new NotImplementedException();
    }

    public void Init()
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

    skipautoload:
    }
}
