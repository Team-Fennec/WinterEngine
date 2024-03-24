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
    private static readonly ILog m_Log = LogManager.GetLogger(typeof(Engine));

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
        InputSystem.Init();

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

        // add close input
        InputAction exitAction = new InputAction("ExitGame");
        exitAction.AddBinding(Key.Escape);

        InputSystem.RegisterAction()

        // create gameconsole panel
        m_ImGuiPanels.Add(new UIGameConsole());

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
            Renderer.ImGuiController.Update(deltaTime, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.
            InputSystem.UpdateEvents(snapshot);

            // when pressing escape, close the game
            if (InputSystem.ActionCheckPressed("ExitGame"))
            {
                Device.Window.Close();
                break; // always break even if threaded.
            }

#if HAS_PROFILING
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

    public static void Shutdown()
    {
        m_Log.Info("Beginning Engine Shutdown");
        IsRunning = false;

        m_GameInstance.Shutdown();
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
}
