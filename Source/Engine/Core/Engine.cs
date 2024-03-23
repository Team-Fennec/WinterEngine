using Veldrid;
using Veldrid.ImageSharp;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using WinterEngine.RenderSystem;
using WinterEngine.SceneSystem;
using WinterEngine.Resource;
using WinterEngine.Gui;
using WinterEngine.Gui.DevUI;
using Veldrid.Sdl2;

using static WinterEngine.Localization.StringTools;

namespace WinterEngine.Core;

public class Engine
{
	// Logger
    private static readonly ILog log = LogManager.GetLogger(typeof(Engine));

	private static int _returnCode = 0;

	private static bool _showImGuiDemoWindow = true;
    private static bool _showAnotherWindow = false;

	private static Assembly gameAssembly;
	private static GameModule gameInstance;

    private static List<ImGuiPanel> imGuiPanels = new List<ImGuiPanel>();

    public static bool IsRunning = false;

    public static void PreInit() {
        log.Info("Adding engine resources");
        // adds the engine resources and starts up certain engine systems
        ResourceManager.AddProvider(new Providers.DirectoryProvider("engine"));
        // todo(engine): get system lang and load the corresponding translation (or try to)
        TranslationManager.AddTranslation("engine_english.txt");
    }

	public static void Init(string gameDir) {
		log.Info("Initializing Engine...");

		log.Info("Reading Gameinfo...");

		JsonValue gameInfoData = HjsonValue.Load(Path.Combine(gameDir, "gameinfo.hjson"));
		
		JsonValue gameProperName;
		gameInfoData.Qo().TryGetValue("name", out gameProperName);

		JsonValue resDirs;
		gameInfoData.Qo().TryGetValue("resourcePaths", out resDirs);
		foreach (JsonValue jValue in resDirs.Qa()) {
			ResourceManager.AddResourceProvider(jValue.Qstr());
		}

		// search for bin dir
		if (Directory.Exists(Path.Combine(gameDir, "bin"))) {
            string execAssemPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // try and load client.dll
            if (File.Exists(Path.Combine(gameDir, "bin", "game.dll"))) {
                gameAssembly = Assembly.LoadFile(Path.Combine(execAssemPath, gameDir, "bin", "game.dll"));
                log.Info("Loaded Game Dll");
            } else {
				Error(TRS("engine.error.load_game_dll_fail"));
            }
		} else {
            Error(TRS("engine.error.no_bin_folder"));
        }

        // spin up the first instance of a client class we find
        gameInstance = (GameModule)gameAssembly.CreateInstance(
			gameAssembly.GetTypes().Where(t => typeof(GameModule).IsAssignableFrom(t)).First().FullName
		);
		gameInstance.Startup();

		Device.Init(gameProperName.Qstr());
		Renderer.Init();

		// create gameconsole panel
		imGuiPanels.Add(new UIGameConsole());
		
		IsRunning = true;
	}

	public static int Run()
	{
		log.Info("Starting Engine loop...");
		var stopwatch = Stopwatch.StartNew();
		float deltaTime = 0f;

		while (Device.Window.Exists)
		{
			deltaTime = stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
			stopwatch.Restart();
			InputSnapshot snapshot = Device.Window.PumpEvents();
			if (!Device.Window.Exists) { break; }
			Renderer.ImGuiController.Update(deltaTime, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

#if HAS_PROFILING
            Profiler.PushProfile("ImGuiUpdate");
#endif
			// imgui stuff
			foreach (ImGuiPanel panel in imGuiPanels) {
				panel.DoLayout();
			}
#if HAS_PROFILING
            Profiler.PopProfile();
#endif
            
			SceneSystem.Update(deltaTime);
			Renderer.Render(deltaTime);
		}

		Shutdown();
		return _returnCode;
    }

	public static void Shutdown() {
		log.Info("Beginning Engine Shutdown");
		IsRunning = false;
        
		gameInstance.Shutdown();
		Renderer.Shutdown();

        log.Info("Engine Shutdown Complete");
	}
    
	public static void Error(string message) {
		log.Error(message);
        unsafe {
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
