using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;
using log4net;
using ImGuiNET;
using ImVGuiNET;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using Hjson;
using System.Reflection;

namespace WinterEngine.Core;

public class Engine
{
	// Logger
    private static readonly ILog log = LogManager.GetLogger(typeof(Engine));

	private static GraphicsDevice _graphicsDevice;
	private static Sdl2Window _window;
	private static CommandList _cl;
	private static ImGuiController _imguiRend;
	private static DeviceBuffer _vertexBuffer;
	private static DeviceBuffer _indexBuffer;
	private static Shader[] _shaders;
	private static Pipeline _pipeline;

	private static bool _showImGuiDemoWindow = true;
    private static bool _showAnotherWindow = false;
	private static Vector3 _clearColor = new Vector3(0.45f, 0.55f, 0.6f);

	public static void Init(string gameDir) {
		log.Info("Initializing Engine...");

		log.Info("Reading Gameinfo...");

		JsonValue gameInfoData = HjsonValue.Load(Path.Combine(gameDir, "gameinfo.hjson"));
		
		JsonValue gameProperName;
		gameInfoData.Qo().TryGetValue("name", out gameProperName);

		JsonValue resDirs;
		gameInfoData.Qo().TryGetValue("resourcePaths", out resDirs);
		foreach (JsonValue jValue in resDirs.Qa()) {
			Resource.ResourceManager.AddResourceDirectory(jValue.Qstr());
		}

		// search for progs.dll inside the game folder
		if (File.Exists(Path.Combine(gameDir, "progs.dll"))) {
			string execAssemPath = Assembly.GetExecutingAssembly().Location;
			Assembly.LoadFile(Path.Combine(Path.GetDirectoryName(execAssemPath), gameDir, "progs.dll"));
			log.Info("Loaded progs.dll for game");
		}

		log.Info("Initializing Veldrid SDL2 Window...");
		WindowCreateInfo windowCI = new WindowCreateInfo()
		{
			X = 100,
			Y = 100,
			WindowWidth = 960,
			WindowHeight = 540,
			WindowTitle = gameProperName.Qstr()
		};
		_window = VeldridStartup.CreateWindow(ref windowCI);

		log.Info("Initializing GraphicsDevice...");
		GraphicsDeviceOptions options = new GraphicsDeviceOptions
		{
			PreferStandardClipSpaceYDirection = true,
			PreferDepthRangeZeroToOne = true
		};
		_graphicsDevice = VeldridStartup.CreateGraphicsDevice(_window, options);
		_cl = _graphicsDevice.ResourceFactory.CreateCommandList();

		log.Info("Initializing ImGui...");

		_imguiRend = new ImGuiController(
			_graphicsDevice,
			_graphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
			_window.Width,
			_window.Height
		);
		ImVGui.StyleEngineTools();

		log.Info("Creating Veldrid Resources...");
		CreateResources();
	}

	public static int Run()
	{
		log.Info("Starting Engine loop...");
		var stopwatch = Stopwatch.StartNew();
		float deltaTime = 0f;

		while (_window.Exists)
		{
			deltaTime = stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
			stopwatch.Restart();
			InputSnapshot snapshot = _window.PumpEvents();
			if (!_window.Exists) { break; }
			_imguiRend.Update(deltaTime, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

			// imgui stuff
			ImGui.Checkbox("show vgui demo", ref _showAnotherWindow);
			ImGui.Checkbox("show imgui demo", ref _showImGuiDemoWindow);

			if (_showImGuiDemoWindow) {
				ImGui.ShowDemoWindow(ref _showImGuiDemoWindow);
			}

			if (_showAnotherWindow) {
				ImVGui.ShowDemoWindow(ref _showAnotherWindow);
			}

			_cl.Begin();
			_cl.SetFramebuffer(_graphicsDevice.MainSwapchain.Framebuffer);

			_cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));

			// Send calls to renderer main to render everything
			_cl.SetVertexBuffer(0, _vertexBuffer);
			_cl.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
			_cl.SetPipeline(_pipeline);
			_cl.DrawIndexed(
				indexCount: 6,
				instanceCount: 2,
				indexStart: 0,
				vertexOffset: 0,
				instanceStart: 0
			);

			_imguiRend.Render(_graphicsDevice, _cl);

			_cl.End();
			_graphicsDevice.SubmitCommands(_cl);
			_graphicsDevice.SwapBuffers(_graphicsDevice.MainSwapchain);
		}

		return 0;
	}

	public static void Shutdown() {
		log.Info("Beginning Engine Shutdown");

		_graphicsDevice.WaitForIdle();

		log.Info("Disposing Veldrid Resources...");

		_pipeline.Dispose();
		foreach (Shader shader in _shaders)
		{
			shader.Dispose();
		}
		_cl.Dispose();
		_vertexBuffer.Dispose();
		_indexBuffer.Dispose();
		_graphicsDevice.Dispose();
		_imguiRend.Dispose();

		log.Info("Engine Shutdown Complete");
	}

	static void CreateResources() {
		ResourceFactory factory = _graphicsDevice.ResourceFactory;

		VertexPositionColor[] quadVertices =
		{
			new VertexPositionColor(new Vector2(-0.75f, 0.75f), RgbaFloat.Red),
			new VertexPositionColor(new Vector2(0.75f, 0.75f), RgbaFloat.Green),
			new VertexPositionColor(new Vector2(-0.75f, -0.75f), RgbaFloat.Blue),
			new VertexPositionColor(new Vector2(0.75f, -0.75f), RgbaFloat.Yellow)
		};

		ushort[] quadIndices = { 0, 1, 2, 0, 2, 3 };

		_vertexBuffer = factory.CreateBuffer(new BufferDescription(4 * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
		_indexBuffer = factory.CreateBuffer(new BufferDescription(6 * sizeof(ushort), BufferUsage.IndexBuffer));

		_graphicsDevice.UpdateBuffer(_vertexBuffer, 0, quadVertices);
		_graphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);

		VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
		new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
		new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));

		// load shader code
		Resource.ShaderResource basicShader = new("UnlitVertexColored");

		ShaderDescription vertexShaderDesc = new ShaderDescription(
			ShaderStages.Vertex,
			Encoding.UTF8.GetBytes(basicShader.VertexCode),
			"main");
		ShaderDescription fragmentShaderDesc = new ShaderDescription(
			ShaderStages.Fragment,
			Encoding.UTF8.GetBytes(basicShader.FragmentCode),
			"main");

		_shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

		GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
		pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

		pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
			depthTestEnabled: true,
			depthWriteEnabled: true,
			comparisonKind: ComparisonKind.LessEqual
		);

		pipelineDescription.RasterizerState = new RasterizerStateDescription(
			cullMode: FaceCullMode.Back,
			fillMode: PolygonFillMode.Solid,
			frontFace: FrontFace.Clockwise,
			depthClipEnabled: true,
			scissorTestEnabled: false
		);

		pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleList;
		pipelineDescription.ResourceLayouts = System.Array.Empty<ResourceLayout>();

		pipelineDescription.ShaderSet = new ShaderSetDescription(
			vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
			shaders: _shaders
		);

		pipelineDescription.Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription;

		_pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
	}
}

struct VertexPositionColor
{
    public Vector2 Position; // This is the position, in normalized device coordinates.
    public RgbaFloat Color; // This is the color of the vertex.
    public VertexPositionColor(Vector2 position, RgbaFloat color)
    {
        Position = position;
        Color = color;
    }
    public const uint SizeInBytes = 24;
}
