using ImGuiNET;
using System.Diagnostics;
using System.Reflection;
using ValveKeyValue;
using Veldrid;
using Veldrid.Sdl2;
#if HAS_PROFILING
using WinterEngine.Diagnostics;
#endif
using WinterEngine.Gui;
using WinterEngine.Gui.DevUI;
using WinterEngine.InputSystem;
using WinterEngine.Localization;
using WinterEngine.RenderSystem;
using WinterEngine.Resource;
using WinterEngine.SceneSystem;
using static WinterEngine.Localization.StringTools;

namespace WinterEngine.Core;

public class Engine
{
    // Logger
    private static readonly ILog m_Log = LogManager.GetLogger("Engine");

    private static int m_ReturnCode = 0;

    private static Assembly m_GameAssembly;
    private static GameModule m_GameInstance;

    private static List<ImGuiPanel> m_ImGuiPanels = new List<ImGuiPanel>();

    public static bool IsRunning = false;

    public static void PreInit()
    {
        m_Log.Info("Adding engine resources");
        // adds the engine resources and starts up certain engine systems
        ResourceManager.AddProvider(new Resource.Providers.DirectoryProvider("engine"));
        // todo(engine): get system lang and load the corresponding translation (or try to)
        TranslationManager.AddTranslation("engine_english.txt");
    }

    public static void Init(string gameDir)
    {
        m_Log.Info("Initializing Engine...");

        m_Log.Info("Reading Gameinfo...");

        var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
        KVObject gameInfoData = kv.Deserialize(File.Open(Path.Combine(gameDir, "gameinfo.gi"), FileMode.Open));

        KVValue gameProperName = gameInfoData["name"];

        foreach (KVObject dirItem in (IEnumerable<KVObject>)gameInfoData["ResourcePaths"])
        {
            switch (dirItem.Name)
            {
                case "dir":
                    ResourceManager.AddProvider(new Resource.Providers.DirectoryProvider(dirItem.Value.ToString()));
                    break;
                case "vpk":
                    ResourceManager.AddProvider(new Resource.Providers.VpkProvider(dirItem.Value.ToString()));
                    break;
                default:
                    throw new ArgumentException($"Invalid resource provider type {dirItem.Name}.");
            }
        }

        Device.Init(gameProperName.ToString());
        Renderer.Init();
        InputManager.Init();

        // Setup game console
        InputAction conAction = new InputAction("Console");
        conAction.AddBinding(Key.Tilde);
        InputManager.RegisterAction(conAction);
        GameConsole.RegisterCommand(new ConsoleCommands.HelpCommand());
        GameConsole.RegisterCommand(new ConsoleCommands.QuitCommand());

        // load up the game now that we're initialized
        // search for bin dir
        if (Directory.Exists(Path.Combine(gameDir, "bin")))
        {
            string execAssemPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // try and load client.dll
            if (File.Exists(Path.Combine(gameDir, "bin", "game.dll")))
            {
                m_GameAssembly = Assembly.LoadFile(Path.Combine(execAssemPath, gameDir, "bin", "game.dll"));
                m_Log.Info("Loaded Game Dll");
            }
            else
            {
                Error(TRS("engine.error.load_game_dll_fail"));
            }
        }
        else
        {
            Error(TRS("engine.error.no_bin_folder"));
        }

        m_GameInstance = (GameModule)m_GameAssembly.CreateInstance(
            m_GameAssembly.GetTypes().Where(t => typeof(GameModule).IsAssignableFrom(t)).First().FullName
        );
        m_GameInstance.Startup();

        
        // Add gui game console
        m_ImGuiPanels.Add(new UIGameConsole());

#if HAS_PROFILING
        InputAction profAction = new InputAction("Profiler");
        profAction.AddBinding(Key.F1);
        InputManager.RegisterAction(profAction);

        m_ImGuiPanels.Add(new ProfilerPanel());
#endif

        IsRunning = true;
    }

    public static int Run()
    {
        m_Log.Info("Starting Engine loop...");
        var stopwatch = Stopwatch.StartNew();
        float deltaTime = 0f;

        while (Device.Window.Exists)
        {
            deltaTime = stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
            stopwatch.Restart();
            InputSnapshot snapshot = Device.Window.PumpEvents();
            if (!Device.Window.Exists)
            { break; }

            InputManager.ProcessInputs = (!ImGui.GetIO().WantCaptureKeyboard && !ImGui.GetIO().WantCaptureMouse);
            InputManager.UpdateEvents(snapshot);

            Renderer.ImGuiController.Update(deltaTime, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

            if (InputManager.ActionCheckPressed("Console"))
            { SetPanelVisible("game_console", true); }

#if HAS_PROFILING
            if (InputManager.ActionCheckPressed("Profiler"))
            { SetPanelVisible("engine_profiler", !GetPanelVisible("engine_profiler")); }

            Profiler.PushProfile("ImGuiUpdate");
#endif
            // imgui stuff
            foreach (ImGuiPanel panel in m_ImGuiPanels)
            {
                panel.DoLayout();
            }
#if HAS_PROFILING
            Profiler.PopProfile();
#endif

            SceneManager.Update(deltaTime);
            Renderer.Render(deltaTime);
        }

        Shutdown();
        return m_ReturnCode;
    }

    public static void SetPanelVisible(string ID, bool v)
    {
        foreach (ImGuiPanel panel in m_ImGuiPanels)
        {
            if (panel.ID == ID)
            {
                panel.Visible = v;
                return;
            }
        }
        m_Log.Error($"No panel was found with ID {ID}");
    }

    public static bool GetPanelVisible(string ID)
    {
        foreach (ImGuiPanel panel in m_ImGuiPanels)
        {
            if (panel.ID == ID)
            {
                return panel.Visible;
            }
        }
        m_Log.Warn($"No panel was found with ID {ID}");
        return false;
    }

    public static void Shutdown()
    {
        m_Log.Info("Beginning Engine Shutdown");
        IsRunning = false;

        m_GameInstance.Shutdown();
        SceneManager.Shutdown();
        Renderer.Shutdown();

        m_Log.Info("Engine Shutdown Complete");
    }

    public static void Error(string message)
    {
        m_Log.Error(message);
        unsafe
        {
            Sdl2Native.SDL_ShowSimpleMessageBox(
                SDL_MessageBoxFlags.Error,
                "Winter Engine",
                $"Engine Error:\n{message}",
                null
            );
        }
        // Just fucking exit lol
        Environment.Exit(1);
    }

    public static void ExecuteCommand(string input)
    {
        string[] inpSplit = input.Split(" ");

        if (GameConsole.cmdList.ContainsKey(inpSplit[0]))
        {
            GameConsole.cmdList.TryGetValue(inpSplit[0], out var command);
            string[] args = inpSplit.Skip(1).ToArray();
            // do command
            command.Exec(args);
        }
    }
}
