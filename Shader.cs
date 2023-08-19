using System.IO;
using System.Text;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using DoomGame.Debug;
using DoomGame.Resource.Types;
using Newtonsoft.Json;

namespace DoomGame.Rendering;

public class Shader
{
	public int Handle;
	private bool disposedValue = false;

	public Shader(string shaderName)
	{
		// Shader handles
		int VertexShader, FragmentShader;

		string shaderResourceSource = File.ReadAllText($"shaders/{shaderName}.shader");
		ShaderResource shaderResource = JsonConvert.DeserializeObject<ShaderResource>(shaderResourceSource);

		// TODO: Move game data into a dedicated game data folder
		string VertexShaderSource = File.ReadAllText($"shaders/{shaderResource.vertexShader}");
		string FragmentShaderSource = File.ReadAllText($"shaders/{shaderResource.fragmentShader}");

		#region Generate/Compile shaders

		VertexShader = GL.CreateShader(ShaderType.VertexShader);
		GL.ShaderSource(VertexShader, VertexShaderSource);

		FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
		GL.ShaderSource(FragmentShader, FragmentShaderSource);

		// Vertex Shader
		CompileShader(VertexShader);

		// Fragment Shader
		CompileShader(FragmentShader);

		#endregion

		#region Create shader program

		Handle = GL.CreateProgram();

		GL.AttachShader(Handle, VertexShader);
		GL.AttachShader(Handle, FragmentShader);

		GL.LinkProgram(Handle);

		GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
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

	~Shader()
	{
		if (!disposedValue)
		{
			Logger.Log("Shader", "GPU Resource leak! Did you forget to call Dispose()?", LogType.Warning);
		}
	}

	private void CompileShader(int shader)
	{
		GL.CompileShader(shader);

		GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
		if (success == 0)
		{
			string infoLog = GL.GetShaderInfoLog(shader);
			Logger.Log("Shader", $"Failed to compile shader: {infoLog}", LogType.Error);
		}
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

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}