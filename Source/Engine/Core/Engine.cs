using Veldrid;
using ImGuiNET;
using System.Diagnostics;
using System.Reflection;
using WinterEngine.RenderSystem;
using WinterEngine.SceneSystem;
using WinterEngine.Resource;
using WinterEngine.Gui;
using WinterEngine.Gui.DevUI;
using Veldrid.Sdl2;
using static WinterEngine.Localization.StringTools;
using WinterEngine.Localization;
using ValveKeyValue;
using System.IO;

namespace WinterEngine.Core;

public class Engine
{
    // Logger
    private static readonly ILog m_Log = LogManager.GetLogger(typeof(Engine));

    private static int m_ReturnCode = 0;

    private static bool m_ShowImGuiDemoWindow = true;
    private static bool m_ShowAnotherWindow = false;

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
        KVObject gameInfoData = kv.Deserialize(File.Open(Path.Combine(gameDir, "gameinfo.txt"), FileMode.Open));

        KVValue gameProperName = gameInfoData["name"];

        foreach (KVObject dirItem in (IEnumerable<KVObject>)gameInfoData["ResourcePaths"])
        {
            switch (dirItem.Name)
            {
                case "dir":
                    ResourceManager.AddProvider(new Resource.Providers.DirectoryProvider(dirItem.Value.ToString()));
                    break;
                default:
                    throw new ArgumentException($"Invalid resource provider type {dirItem.Name}.");
            }
        }

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

        // spin up the first instance of a client class we find
        m_GameInstance = (GameModule)m_GameAssembly.CreateInstance(
            m_GameAssembly.GetTypes().Where(t => typeof(GameModule).IsAssignableFrom(t)).First().FullName
        );
        m_GameInstance.Startup();

        Device.Init(gameProperName.ToString());
        Renderer.Init();

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
