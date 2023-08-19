using System.IO;
using System.Text;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using DoomGame.Debug;

namespace DoomGame.Rendering;

public class Shader
{
	int Handle;
	private bool disposedValue = false;

	public Shader(string vertexPath, string fragmentPath)
	{
		// Shader handles
		int VertexShader, FragmentShader, success;

		string VertexShaderSource = File.ReadAllText(vertexPath);
		string FragmentShaderSource = File.ReadAllText(fragmentPath);

		#region Generate shaders and bind source code

		VertexShader = GL.CreateShader(ShaderType.VertexShader);
		GL.ShaderSource(VertexShader, VertexShaderSource);

		FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
		GL.ShaderSource(FragmentShader, FragmentShaderSource);

		#endregion

		#region Compile Shaders and check for errors
		// Vertex Shader
		GL.CompileShader(VertexShader);

		GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out success);
		if (success == 0)
		{
			string infoLog = GL.GetShaderInfoLog(VertexShader);
			Logger.Log("Shader", $"Error in Vertex Shader: {infoLog}", LogType.Error);
		}

		// Fragment Shader
		GL.CompileShader(FragmentShader);

		GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out success);
		if (success == 0)
		{
			string infoLog = GL.GetShaderInfoLog(FragmentShader);
			Logger.Log("Shader", $"Error in Fragment Shader: {infoLog}", LogType.Error);
		}

		#endregion

		#region Create shader program

		Handle = GL.CreateProgram();

		GL.AttachShader(Handle, VertexShader);
		GL.AttachShader(Handle, FragmentShader);

		GL.LinkProgram(Handle);

		GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out success);
		if (success == 0)
		{
			string infoLog = GL.GetProgramInfoLog(Handle);
			Logger.Log("Shader", $"Error in Shader Program: {infoLog}", LogType.Error);
		}

		#endregion

		Logger.Log("Shader", "Successfully compiled Shader program", LogType.Info);

		GL.DetachShader(Handle, VertexShader);
		GL.DetachShader(Handle, FragmentShader);
		GL.DeleteShader(VertexShader);
		GL.DeleteShader(FragmentShader);
	}

	public void Use()
	{
		GL.UseProgram(Handle);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			Logger.Log("Shader", "Disposing shader program...", LogType.Info);

			GL.DeleteProgram(Handle);

			disposedValue = true;
		}
	}

	~Shader()
	{
		if (!disposedValue)
		{
			Logger.Log("Shader", "GPU Resource leak! Did you forget to call Dispose()?", LogType.Warning);
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}