using System.IO;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Newtonsoft.Json;
using DoomGame.Resource.Types;
using DoomGame.Rendering;
using DoomGame.Debug;

namespace DoomGame.Main;

// Kinda hate that game and gamewindow are a combined entity but wygd
public class Game : GameWindow
{
	#region debug object

	ModelResource model;
	Shader shader;

	private Stopwatch _timer;

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
		model = LoadMDL("rectangle.model");

		// VBOs
		VertexBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
		// Upload vertices to the buffer
		GL.BufferData(BufferTarget.ArrayBuffer, model.vertices.Length * sizeof(float), model.vertices, BufferUsageHint.StaticDraw);
	
		// Linking vertex attributes
		VertexArrayObject = GL.GenVertexArray();

		GL.BindVertexArray(VertexArrayObject);

		GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
		GL.EnableVertexAttribArray(0);

		ElementBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
		GL.BufferData(BufferTarget.ElementArrayBuffer, model.indices.Length * sizeof(uint), model.indices, BufferUsageHint.StaticDraw);

		// Shaders
		shader = new Shader("basic_uc");

		shader.Use();

		#endregion

		_timer = new();
		_timer.Start();
	}

	// Loads vertices and indices from a json file
	private ModelResource LoadMDL(string filename)
	{
		if (!File.Exists($"models/{filename}"))
		{
			Logger.Log("Model", $"Could not find model file: \"{filename}\"", LogType.Warning);
		}

		string modelSource = File.ReadAllText($"models/{filename}");
		ModelResource modelData = JsonConvert.DeserializeObject<ModelResource>(modelSource);

		Logger.Log("Model", $"Loaded model data \"{filename}\"", LogType.Info);

		return modelData;
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

		double timeValue = _timer.Elapsed.TotalSeconds;
		float greenValue = (float)Math.Sin(timeValue) / 2.0f + 0.5f;
		int vertexColorLocation = GL.GetUniformLocation(shader.Handle, "ourColor");
		GL.Uniform4(vertexColorLocation, 0.0f, greenValue, 0.0f, 1.0f);
		
		GL.BindVertexArray(VertexArrayObject);
		GL.DrawElements(PrimitiveType.Triangles, model.indices.Length, DrawElementsType.UnsignedInt, 0);

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