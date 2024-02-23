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
using WinterEngine.Rendering;
using WinterEngine.Actors;
using WinterEngine.Resource;

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
			Resource.ResourceManager.AddResourceProvider(jValue.Qstr());
		}

		// search for progs.dll inside the game folder
		if (Directory.Exists(Path.Combine(gameDir, "bin"))) {
		    // get listing of files inside directory and load them
			/*string execAssemPath = Assembly.GetExecutingAssembly().Location;
			Assembly.LoadFile(Path.Combine(Path.GetDirectoryName(execAssemPath), gameDir, "progs.dll"));
			log.Info("Loaded");*/
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
		//ImVGui.StyleEngineTools();

		modelData = new Md3Model("snap");

		log.Info("Creating Veldrid Resources...");
		CreateResources();
	}

	static Md3Model modelData;
	static void DisplayModelData(Md3Model data) {
		if (ImGui.Begin("Model Info")) {
			ImGui.Text(data.Header.name);
			ImGui.Text($"{data.Header.version}");
			ImGui.Separator();
			ImGui.Text($"Frames: {data.Header.numFrames}");
			ImGui.Text($"Tags: {data.Header.numTags}");
			ImGui.Text($"Surfaces: {data.Header.numSurfaces}");
			ImGui.Text($"Skins: {data.Header.numSkins}");
			ImGui.Separator();
			// frames
			if (ImGui.CollapsingHeader("Frames")) {
				int count = 0;
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
				foreach (Md3Frame frame in data.Frames) {
					if (ImGui.CollapsingHeader($"Frame {count} ({frame.name})##root_frames_frame_{count}")) {
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
						ImGui.Text($"Min Bounds: {frame.minBounds.X},{frame.minBounds.Y},{frame.minBounds.Z}");
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
						ImGui.Text($"Max Bounds: {frame.maxBounds.X},{frame.maxBounds.Y},{frame.maxBounds.Z}");
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
						ImGui.Text($"Local Origin: {frame.localOrigin.X},{frame.localOrigin.Y},{frame.localOrigin.Z}");
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
						ImGui.Text($"Radius: {frame.radius}");
					}
					ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
					count++;
				}
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 8);
			}

			if (ImGui.CollapsingHeader("Tags")) {
				int count = 0;
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
				foreach (Md3Tag tag in data.Tags) {
					if (ImGui.CollapsingHeader($"Tag {count} ({tag.name})##root_tags_tag_{count}")) {
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
						ImGui.Text($"Origin: {tag.origin.X},{tag.origin.Y},{tag.origin.Z}");
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
						ImGui.Text($"Axis 1: {tag.axis[0].X},{tag.axis[0].Y},{tag.axis[0].Z}");
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
						ImGui.Text($"Axis 2: {tag.axis[1].X},{tag.axis[1].Y},{tag.axis[1].Z}");
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
						ImGui.Text($"Axis 3: {tag.axis[2].X},{tag.axis[2].Y},{tag.axis[2].Z}");
					}
					ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
					count++;
				}
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 8);
			}

			if (ImGui.CollapsingHeader("Surfaces")) {
				int count = 0;
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
				foreach (Md3Surface surf in data.Surfaces) {
					if (ImGui.CollapsingHeader($"Surface {count} ({surf.name})##root_surfs_surf_{count}")) {
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
						ImGui.Text($"Frames: {surf.numFrames}");
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
						ImGui.Text($"Shaders: {surf.numShaders}");
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
						ImGui.Text($"Verticies: {surf.numVerts}");
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
						ImGui.Text($"Triangles: {surf.numTriangles}");
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
						ImGui.Separator();
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);

						if (ImGui.CollapsingHeader($"Shaders##root_surf_{count}_shaders")) {
							int sCount = 0;
							ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
							foreach (Md3Shader shader in surf.shaders) {
								if (ImGui.CollapsingHeader($"Shader {sCount}##root_surf_{count}_shaders_shader_{sCount}")) {
									ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
									ImGui.Text(shader.name);
									ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
									ImGui.Text($"{shader.shaderIndex}");
								}
								sCount++;
								ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
							}
							ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 16);
						}
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);

						if (ImGui.CollapsingHeader($"Triangles##root_surf_{count}_triangles")) {
							int sCount = 0;
							ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
							foreach (Md3Triangle trig in surf.triangles) {
								ImGui.Text($"{sCount} | Indices: {trig.indexes[0]},{trig.indexes[1]},{trig.indexes[2]}");
								sCount++;
								ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
							}
							ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 8);
						}
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);

						if (ImGui.CollapsingHeader($"UVs##root_surf_{count}_uvs")) {
							int sCount = 0;
							ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
							foreach (Vector2 uv in surf.texCoords) {
								ImGui.Text($"{sCount}: {uv.X},{uv.Y}");
								sCount++;
								ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
							}
							ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 8);
						}
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);

						if (ImGui.CollapsingHeader($"Vertices##root_surf_{count}_vertices")) {
							int sCount = 0;
							ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
							for (int i = 0; i < (surf.numVerts * surf.numFrames); i += surf.numVerts) {
								if (ImGui.CollapsingHeader($"Frame {sCount}##root_surf_{count}_verts_vertframe_{sCount}")) {
									int vCount = 0;
									ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
									for (int f = i; f < i + surf.numVerts; f++) {
										Md3Vertex vertex = surf.vertices[f];
										ImGui.Text($"{vCount} | Position: {vertex.x},{vertex.y},{vertex.z} Normal: {vertex.normal}");
										vCount++;
										ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
									}
									ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 16);
								}
								sCount++;
								ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
							}
							ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 8);
						}
					}
					ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
					count++;
				}
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 8);
			}
			
			ImGui.End();
		}
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
			ImGui.Checkbox("show imgui demo", ref _showImGuiDemoWindow);

			if (_showImGuiDemoWindow) {
				ImGui.ShowDemoWindow(ref _showImGuiDemoWindow);
			}
			DisplayModelData(modelData);

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
