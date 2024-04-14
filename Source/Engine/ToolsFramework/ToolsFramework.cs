using System;
using System.Collections.Generic;
using ValveKeyValue;
using WinterEngine.Core;
using Veneer;
using WinterEngine.ToolsFramework.Gui;
using WinterEngine.Utilities;
using System.Reflection;
using Gtk;

namespace WinterEngine.ToolsFramework;

public static class ToolsFramework
{
    private static readonly ILog log = LogManager.GetLogger("ToolsFramework");

    static List<EngineTool> m_EngineTools = new List<EngineTool>();
    static EngineTool m_CurrentTool;
    static bool m_ToolsActive = true; // default to true

    public static Application m_gtkApplication; // useed to handle GTKsharp within the tools framework
    static Task m_gtkLoop;
    static GtkSplashWindow m_Splash;

    public static EngineTool GetCurrentTool() => m_CurrentTool;
    public static IReadOnlyList<EngineTool> GetToolList() => m_EngineTools;

    public static void Init()
    {
        log.Info("Initializing Engine Tools...");

        GameConsole.RegisterCommand(new LoadToolCommand());

        Application.Init();

        m_gtkApplication = new Application("org.Fennec.WinterTools", GLib.ApplicationFlags.None);
        m_gtkApplication.Register(GLib.Cancellable.Current);

        // create splash window
        m_Splash = new GtkSplashWindow();
        m_gtkApplication.AddWindow(m_Splash);
        m_Splash.SetStatusText("Loading Tools Framework");
        m_Splash.Show();

        // run iteration to show the splash
        Application.RunIteration();

        // search for an enginetools.txt file around us
        if (!File.Exists("enginetools.vdf"))
        {
#if VERBOSE_LOGGING
            log.Info("No enginetools.txt found.");
#endif
        }

        // run file through our kv interpreter
        var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
        KVObject engineToolsList = kv.Deserialize(File.OpenRead("enginetools.vdf"));

        // go through the entries and add the tools
        foreach (KVObject toolObj in (IEnumerable<KVObject>)engineToolsList.Value)
        {
            Engine.ExecuteCommand($"tool_load {toolObj.Value}");
        }

        // add panels
        GuiManager.AddPanel(new ModuleLoadPanel());
        GuiManager.AddPanel(new ToolRootPanel());

        m_Splash.Hide();
    }

    public static void Shutdown()
    {
        Application.Quit();
    }

    public static async void Update()
    {
        // disgusting hack: this should not work how does this NOT cause memory violations and corruption
        await Task.Run(Application.RunIteration);
    }

    public static void SwitchTool(string name)
    {
        foreach (EngineTool tool in m_EngineTools)
        {
            if (tool.ToolName == name)
            {
                tool.OnEnable();
                m_CurrentTool = tool;
                return;
            }
        }
        log.Error($"No tool by name '{name}' is registered");
    }

    public static void RegisterTool(EngineTool instance)
    {
        m_EngineTools.Add(instance);
    }
}

internal sealed class LoadToolCommand : ConCmd
{
    public override string Command => "tool_load";
    public override string Description => "Loads a tool from the provided name";
    public override CmdFlags Flags => CmdFlags.None;

    public override void Exec(string[] args)
    {
        if (args.Length < 1)
        { return; }

        // todo: load tool command
        // load up the game now that we're initialized
        // search for bin dir
        if (Directory.Exists(Path.Combine("bin", "tools")))
        {
            string execAssemPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // try and load client.dll
            if (File.Exists(Path.Combine("bin", "tools", $"{args[0]}.dll")))
            {
                Assembly toolAssem = Assembly.LoadFile(Path.Combine(execAssemPath, "bin", "tools", $"{args[0]}.dll"));
                var toolInstance = (EngineTool)toolAssem.CreateInstance(
                    toolAssem.GetTypes().Where(t => typeof(EngineTool).IsAssignableFrom(t)).First().FullName
                );
                toolInstance.Init();
                ToolsFramework.RegisterTool(toolInstance);
                ToolsFramework.SwitchTool(toolInstance.ToolName);

                LogManager.GetLogger("Tools").Notice($"Loaded tool {args[0]}");
            }
        }
    }
}
