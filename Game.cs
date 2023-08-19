using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using DoomGame.Rendering;
using DoomGame.Debug;

namespace DoomGame.Main;

// Kinda hate that game and gamewindow are a combined entity but wygd
public class Game : GameWindow
{
	#region Debug Triangle (My first ever thing displayed)

	float[] vertices = {
		-0.5f, -0.5f, 0.0f, // Bottom-left
		 0.5f, -0.5f, 0.0f, // Bottom-right
		-0.5f,  0.5f, 0.0f  // Top
	};

	Shader shader;

	#endregion

	int VertexBufferObject;
	int VertexArrayObject;

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

		// Shaders
		shader = new Shader("shaders/v_basic.vsh", "shaders/f_basic.fsh");

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
		GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

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