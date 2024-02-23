using log4net;
using ImGuiNET;
using ImVGuiNET;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using Hjson;
using System.Reflection;
using WinterEngine.Rendering;
using WinterEngine.Gui;
using WinterEngine.Resource;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WinterEngine.Core;

public class Engine : Game
{
	// Logger
    private static readonly ILog log = LogManager.GetLogger(typeof(Engine));

	public static GraphicsDeviceManager graphicsDeviceManager { get; private set; }
	private SpriteBatch _spriteBatch;
	private ImGuiRenderer _imGuiRenderer;

	private static bool _showImGuiDemoWindow = true;
    private static bool _showAnotherWindow = false;

	private string gameDir;

	public List<Actors.BaseActor> actors = new List<Actors.BaseActor>();
	public List<ImguiPanel> imguiPanels = new List<ImguiPanel>();

	public Engine(string gameDir) {
		log.Info("Initializing Engine...");

		graphicsDeviceManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = gameDir;
        IsMouseVisible = true;
		this.gameDir = gameDir;
		ResourceManager.Init(Content);
	}

	protected override void Initialize()
    {
		log.Info("Reading Gameinfo...");

		JsonValue gameInfoData = HjsonValue.Load(Path.Combine(gameDir, "gameinfo.hjson"));
		
		JsonValue gameProperName;
		gameInfoData.Qo().TryGetValue("name", out gameProperName);

		log.Info("Adding resource providers");

		JsonValue resDirs;
		gameInfoData.Qo().TryGetValue("resourcePaths", out resDirs);
		foreach (JsonValue jValue in resDirs.Qa()) {
			JsonObject resProv = jValue.Qo();
			JsonValue path;
			JsonValue format;

			resProv.TryGetValue("path", out path);
			resProv.TryGetValue("format", out format);

			if (format.Qstr() == "snowpack") {
				ResourceManager.AddResourceProvider(path.Qstr(), ResourceFormat.Snowpack);
			} else if (format.Qstr() == "folder") {
				ResourceManager.AddResourceProvider(path.Qstr(), ResourceFormat.Folder);
			}
		}

		// search for assemblies in the bin folder
		if (Directory.Exists(Path.Combine(gameDir, "bin"))) {
			// Add any assemblies we find in here
			foreach (string fileName in Directory.EnumerateFiles(Path.Combine(gameDir, "bin"))) {
				Assembly.LoadFile(fileName);
				log.Info($"Added assembly {Path.GetFileName(fileName)}");
			}
		}

		log.Info("Initializing ImGui...");
		_imGuiRenderer = new ImGuiRenderer(this);
		_imGuiRenderer.RebuildFontAtlas();
		ImVGui.StyleEngineTools();

		imguiPanels.Add(new DebugConsole());

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
		ResourceManager.OpenResource("models/gort.obj").Close();
    }

	protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

		foreach (Actors.BaseActor actor in actors) {
			actor.Think(gameTime);
		}

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

		foreach (Actors.BaseActor actor in actors) {
			actor.Render(gameTime);
		}

		// Call BeforeLayout first to set things up
		_imGuiRenderer.BeforeLayout(gameTime);

		// imgui stuff
		ImGui.Checkbox("show imgui demo", ref _showImGuiDemoWindow);

		if (_showImGuiDemoWindow) {
			ImGui.ShowDemoWindow(ref _showImGuiDemoWindow);
		}

		// Draw our UI
		foreach (ImguiPanel panel in imguiPanels) {
			panel.DrawLayout();
		}

		// Call AfterLayout now to finish up and draw all the things
		_imGuiRenderer.AfterLayout();

        base.Draw(gameTime);
    }
}
