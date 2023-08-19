using System.IO;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using Newtonsoft.Json;

using DoomGame.Rendering;
using DoomGame.Debug;

namespace DoomGame.Main;

// TODO: flesh this out into the proper model class/3d object class
public struct Model3D
{
	public float[] vertices;
	public uint[] indices;
}

// Kinda hate that game and gamewindow are a combined entity but wygd
public class Game : GameWindow
{
	#region debug object

	float[] vertices = {
		 0.5f,  0.5f, 0.0f, // top right
		 0.5f, -0.5f, 0.0f, // bottom right
		-0.5f, -0.5f, 0.0f, // bottom left
		-0.5f,  0.5f, 0.0f  // top left
	};

	uint[] indices = {
		0, 1, 3, // first triange
		1, 2, 3  // second triangle
	};

	Shader shader;

	#endregion

	int VertexBufferObject;
	int VertexArrayObject;
	int ElementBufferObject;

	public Game(NativeWindowSettings windowSettings)
	: base(GameWindowSettings.Default, windowSettings)
	{
		Logger.Log("Game", "Constructed GameWindow", LogType.Info);
	}

	protected override void OnLoad()
	{
		base.OnLoad();

		GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);

		// TODO: add other loading code here

		#region Debug Triangle
		// TODO: move this code to some like dedicated rendering setup

		// Load Model
		LoadMDL("rectangle.json");

		// VBOs
		VertexBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
		// Upload vertices to the buffer
		GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
	
		// Linking vertex attributes
		VertexArrayObject = GL.GenVertexArray();

		GL.BindVertexArray(VertexArrayObject);

		GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
		GL.EnableVertexAttribArray(0);

		ElementBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
		GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

		// Shaders
		shader = new Shader("shaders/v_basic.vsh", "shaders/f_basic.fsh");

		shader.Use();

		#endregion
	}

	// Loads vertices and indices from a json file
	private void LoadMDL(string filename)
	{
		if (!File.Exists($"models/{filename}"))
		{
			Logger.Log("Model", $"Could not find model file: \"{filename}\"", LogType.Warning);
			return;
		}

		string modelSource = File.ReadAllText($"models/{filename}");
		Model3D modelData = JsonConvert.DeserializeObject<Model3D>(modelSource);

		Logger.Log("Model", $"Loaded model data \"{filename}\"", LogType.Info);

		indices = modelData.indices;
		vertices = modelData.vertices;
	}

	protected override void OnUnload()
	{
		base.OnUnload();

		shader.Dispose();
	}

	protected override void OnRenderFrame(FrameEventArgs e)
	{
		base.OnRenderFrame(e);

		GL.Clear(ClearBufferMask.ColorBufferBit);

		// TODO: Game rendering code

		// Debug triangle Rendering
		shader.Use();
		GL.BindVertexArray(VertexArrayObject);
		GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

		SwapBuffers();
	}

	protected override void OnResize(ResizeEventArgs e)
	{
		base.OnResize(e);

		GL.Viewport(0, 0, e.Width, e.Height);
	}

	protected override void OnUpdateFrame(FrameEventArgs e)
	{
		base.OnUpdateFrame(e);

		if (KeyboardState.IsKeyDown(Keys.Escape))
		{
			Logger.Log("Game", "Escape pressed. Closing Window...", LogType.Info);
			Close();
		}
	}
}