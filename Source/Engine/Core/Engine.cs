using ImGuiNET;
using System.Diagnostics;
using System.Reflection;
using System.Numerics;
using ValveKeyValue;
using Veldrid;
using Veldrid.Sdl2;
#if HAS_PROFILING
using WinterEngine.Diagnostics;
#endif
using WinterEngine.DevUI;
using WinterEngine.InputSystem;
using WinterEngine.Localization;
using WinterEngine.RenderSystem;
using WinterEngine.Resource;
using WinterEngine.SceneSystem;
using WinterEngine.Utilities;
using static WinterEngine.Localization.StringTools;
using System.Globalization;
using log4net.Layout;
using log4net.Appender;
using Veneer;

#if HAS_MACROS
//#macro ClassLogger(name) private static readonly ILog m_Log = LogManager.GetLogger(name)
#endif

namespace WinterEngine.Core;

public class Engine
{
#if HAS_MACROS
    //ClassLogger("Engine");
#else
    private static readonly ILog m_Log = LogManager.GetLogger("Engine");
#endif

    private static int m_ReturnCode = 0;

    private static Assembly m_GameAssembly;
    private static GameModule m_GameInstance;
    private static FrameTimeAverager m_FTA = new FrameTimeAverager(0.666);

    public static bool IsRunning = false;
    public static string GameDir => m_GameDir;
    private static string m_GameDir = "";

    const string LogPatternBase = "[%level][%logger] %message%newline";

    public static void PreInit()
    {
        // configure logger
        Debug.WriteLine("[ENGINE] Initializing logger");

        #region Log Appenders
        ConsoleExAppender consoleAppender = new ConsoleExAppender()
        {
            Layout = new PatternLayout($"[%date]{LogPatternBase}")
        };
        consoleAppender.ActivateOptions();

        GameConsoleAppender gConsoleAppender = new GameConsoleAppender()
        {
            Layout = new PatternLayout(LogPatternBase)
        };
        gConsoleAppender.ActivateOptions();

        FileAppender fileAppender = new FileAppender()
        {
            Layout = new PatternLayout($"[%date]{LogPatternBase}"),
            Encoding = System.Text.Encoding.Unicode,
            File = Path.Combine("logs", $"runtime_{DateTime.Now.ToString("yyyymmddHHmmss")}.log"),
            AppendToFile = false
        };
        fileAppender.ActivateOptions();

#if DEBUG
        DebugAppender debugAppender = new DebugAppender()
        {
            Layout = new PatternLayout($"[%level] %message%newline")
        };
        debugAppender.ActivateOptions();
        log4net.Config.BasicConfigurator.Configure(new IAppender[] { gConsoleAppender, consoleAppender, fileAppender, debugAppender });
#else
        log4net.Config.BasicConfigurator.Configure(new IAppender[] {gConsoleAppender, consoleAppender, fileAppender});
#endif
        #endregion
        // confirm all logging is working
        m_Log.Info($"WinterEngine {EngineVersion.Version.Major} patch {EngineVersion.Version.Minor} (build {EngineVersion.Build})");

        // add engine resources
        m_Log.Info("Adding engine resources");
        // adds the engine resources and starts up certain engine systems
        ResourceManager.AddProvider(new Resource.Providers.DirectoryProvider("engine"));
        // todo(engine): get system lang and load the corresponding translation (or try to)
        TranslationManager.AddTranslation("engine_english.txt");
    }

    public static void Init(string gameDir)
    {
        m_Log.Info("Reading Gameinfo...");
        m_GameDir = gameDir;

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

        foreach (KVObject langItem in (IEnumerable<KVObject>)gameInfoData["SupportedLanguages"])
        {
            // bruh?
            if (langItem.Value.ToInt32(CultureInfo.CurrentCulture) > 0)
            {
                TranslationManager.AddTranslation($"{gameDir}_{langItem.Name}.txt");
            }
        }

        Device.Init(gameProperName.ToString());
        Renderer.Init();
        InputManager.Init();
        ConfigManager.Init();

        // Setup game console
        InputAction conAction = new InputAction("Console");
        conAction.AddBinding(Key.Tilde);
        InputManager.RegisterAction(conAction);
        GameConsole.RegisterCommand(new ConsoleCommands.HelpCommand());
        GameConsole.RegisterCommand(new ConsoleCommands.QuitCommand());
        GameConsole.RegisterCommand(new ConsoleCommands.ImguiDemoCommand());

        // register config var
        ConfigManager.RegisterCVar("r_showfps", true);
        ConfigManager.RegisterCVar("r_limitfps", true);
        ConfigManager.RegisterCVar("maxfps", 60.0);

        // load up the game now that we're initialized
        // search for bin dir
        if (Directory.Exists(Path.Combine(gameDir, "bin")))
        {
            string execAssemPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // try and load client.dll
            if (File.Exists(Path.Combine(gameDir, "bin", "game.dll")))
            {
                m_GameAssembly = Assembly.LoadFile(Path.Combine(execAssemPath, gameDir, "bin", "game.dll"));
                m_Log.Notice($"Loaded Game Dll for Game {Path.GetDirectoryName(gameDir)}");
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
        m_Log.Info("Registering UIGameConsole panel");
        GuiManager.AddPanel(new UIGameConsole());

#if HAS_PROFILING
        InputAction profAction = new InputAction("Profiler");
        profAction.AddBinding(Key.F1);
        InputManager.RegisterAction(profAction);

        GuiManager.AddPanel(new ProfilerPanel());
#endif
        GuiManager.AddPanel(new VeneerTestPanel());

        IsRunning = true;
    }

    public static int Run()
    {
        m_Log.Info("Starting Engine loop...");
        var stopwatch = Stopwatch.StartNew();
        long previousFrameTicks = 0;

        while (Device.Window.Exists)
        {
            long currentFrameTicks = stopwatch.ElapsedTicks;
            double deltaTime = (currentFrameTicks - previousFrameTicks) / (double)Stopwatch.Frequency;

            while (ConfigManager.GetValue<bool>("r_limitfps") && deltaTime < (1.0 / ConfigManager.GetValue<double>("maxfps")))
            {
                currentFrameTicks = stopwatch.ElapsedTicks;
                deltaTime = (currentFrameTicks - previousFrameTicks) / (double)Stopwatch.Frequency;
            }

            previousFrameTicks = currentFrameTicks;

            InputSnapshot snapshot = Device.Window.PumpEvents();
            if (!Device.Window.Exists)
            { break; }

            m_FTA.AddTime(deltaTime);

            InputManager.ProcessInputs = (!ImGui.GetIO().WantCaptureKeyboard && !ImGui.GetIO().WantCaptureMouse);
            InputManager.UpdateEvents(snapshot);

            Renderer.ImGuiController.Update((float)deltaTime, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

            if (InputManager.ActionCheckPressed("Console"))
            { GuiManager.SetPanelVisible("game_console", true); }

#if HAS_PROFILING
            if (InputManager.ActionCheckPressed("Profiler"))
            { GuiManager.SetPanelVisible("engine_profiler", !GuiManager.GetPanelVisible("engine_profiler")); }

            Profiler.PushProfile("ImGuiUpdate");
#endif

            if (ConfigManager.GetValue<bool>("r_showfps"))
            {
                Vector4 color = Vector4.One;
                if (m_FTA.CurrentAverageFramesPerSecond > 50)
                    color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
                else if (m_FTA.CurrentAverageFramesPerSecond > 30)
                    color = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
                else
                    color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);

                ImGui.GetBackgroundDrawList().AddText(
                    new Vector2(0, Device.Window.Height - 10),
                    ImGui.ColorConvertFloat4ToU32(color),
                    $"FPS: {Math.Round(m_FTA.CurrentAverageFramesPerSecond, 2)}"
                );
            }

            GuiManager.Update();
#if HAS_PROFILING
            Profiler.PopProfile();
#endif

            SceneManager.Update(deltaTime);
            Renderer.Render((float)deltaTime);
        }

        Shutdown();
        return m_ReturnCode;
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
