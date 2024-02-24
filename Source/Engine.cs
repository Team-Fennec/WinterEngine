using Veldrid;
using Veldrid.ImageSharp;
using log4net;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;
using Hjson;
using System.Reflection;
using WinterEngine.Rendering;
using WinterEngine.Rendering.RenderObjects;
using WinterEngine.Actors;
using WinterEngine.Resource;
using Veldrid.Sdl2;

namespace WinterEngine.Core;

public class Engine
{
	// Logger
    private static readonly ILog log = LogManager.GetLogger(typeof(Engine));

	private static ImageSharpTexture _missingTexData;
	private static MeshHandle _snapMesh;
	private static ShaderHandle _snapShader;
	private static ROModel _snapModel;

	private static int _returnCode = 0;

	private static bool _showImGuiDemoWindow = true;
    private static bool _showAnotherWindow = false;

	private static Assembly clientAssembly;
	private static CGameClient clientInstance;

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
			// try and load client.dll
			if (File.Exists(Path.Combine(gameDir, "bin", "client.dll"))) {
                string execAssemPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                clientAssembly = Assembly.LoadFile(Path.Combine(execAssemPath, gameDir, "bin", "client.dll"));
                log.Info("Loaded Client Dll");
            } else {
				Error("Unable to find client.dll in game bin folder");
            }
		} else {
            Error("Unable to find bin folder in game folder");
        }

		// spin up the first instance of a client class we find
		clientInstance = (CGameClient)clientAssembly.CreateInstance(
			clientAssembly.GetTypes().Where(t => typeof(CGameClient).IsAssignableFrom(t)).First().FullName
		);
		clientInstance.ClientStartup();

		Device.Init(gameProperName.Qstr());

		Renderer.Init();

		modelData = new Md3Model("snap");

		// load the snap png data using the resource manager and dispense it to veldrid
		StreamReader snapTex = ResourceManager.OpenResource("materials/models/snap.png");
		_missingTexData = new ImageSharpTexture(snapTex.BaseStream, false);
		snapTex.Close();

		// create snap model to display
		_snapModel = new ROModel();

		ShaderResource unlitTextured = new ShaderResource("UnlitTextured");

		_snapModel.Shader = new ShaderHandle(
			unlitTextured.ShaderName,
			unlitTextured.VertexCode,
			unlitTextured.FragmentCode
		);
		_snapModel.Texture = new TextureHandle(_missingTexData.CreateDeviceTexture(
			Renderer.GraphicsDevice,
			Renderer.GraphicsDevice.ResourceFactory
		));
		_snapModel.Mesh = new MeshHandle(GetModelVertices(), GetModelIndices());
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

		while (Device.Window.Exists)
		{
			deltaTime = stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
			stopwatch.Restart();
			InputSnapshot snapshot = Device.Window.PumpEvents();
			if (!Device.Window.Exists) { break; }
			Renderer.ImGuiController.Update(deltaTime, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

			// imgui stuff
			ImGui.Checkbox("show imgui demo", ref _showImGuiDemoWindow);

			if (_showImGuiDemoWindow) {
				ImGui.ShowDemoWindow(ref _showImGuiDemoWindow);
			}
			DisplayModelData(modelData);

			Renderer.PushRO(_snapModel);
			Renderer.Render();
		}

		Shutdown();
		return _returnCode;
    }

	public static void Shutdown() {
		log.Info("Beginning Engine Shutdown");
        
		clientInstance.ClientShutdown();
		
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
		// begin a clean shutdown of the engine
		_returnCode = 1; // signify an error occurred
		if (Device.Window != null)
			Device.Window.Close();
    }

	static VertexPositionColorTexture[] GetModelVertices() {
		List<VertexPositionColorTexture> vertList = new List<VertexPositionColorTexture>();

		for (int i = 0; i < modelData.Surfaces[0].numVerts; i++) {
			List<Md3Vertex> md3verts = modelData.Surfaces[0].vertices;

			vertList.Add(new VertexPositionColorTexture(
				new Vector3(md3verts[i].x, md3verts[i].y, md3verts[i].z),
				RgbaFloat.White,
				modelData.Surfaces[0].texCoords[i]
			));
		}

		return vertList.ToArray();
	}

	static ushort[] GetModelIndices() {
		List<ushort> indxList = new List<ushort>();

		for (int i = 0; i < modelData.Surfaces[0].numTriangles; i++) {
			indxList.Add((ushort)modelData.Surfaces[0].triangles[i].indexes[0]);
            indxList.Add((ushort)modelData.Surfaces[0].triangles[i].indexes[1]);
            indxList.Add((ushort)modelData.Surfaces[0].triangles[i].indexes[2]);
        }

        return indxList.ToArray();
	}
}
