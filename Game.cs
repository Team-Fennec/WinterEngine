using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using DoomGame.Objects;
using DoomGame.Resource;
using DoomGame.Rendering;
using DoomGame.Debug;

namespace DoomGame.Main;

// Kinda hate that game and gamewindow are a combined entity but wygd
public class Game : GameWindow
{
	#region debug object

	Model model;
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
		model = new(ResourceLoader.LoadModel("rectangle.model"));

		// VBOs
		VertexBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
		// Upload vertices to the buffer
		GL.BufferData(BufferTarget.ArrayBuffer, model.Vertices.Length * sizeof(float), model.Vertices, BufferUsageHint.StaticDraw);
	
		// Linking vertex attributes
		VertexArrayObject = GL.GenVertexArray();

		GL.BindVertexArray(VertexArrayObject);

		GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
		GL.EnableVertexAttribArray(0);

		GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
		GL.EnableVertexAttribArray(1);

		ElementBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
		GL.BufferData(BufferTarget.ElementArrayBuffer, model.Indices.Length * sizeof(uint), model.Indices, BufferUsageHint.StaticDraw);

		// Shaders
		shader = new Shader("basic"); // basic offers a solid color render using the model's vertex colors

		shader.Use();

		#endregion
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
		GL.DrawElements(PrimitiveType.Triangles, model.Indices.Length, DrawElementsType.UnsignedInt, 0);

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