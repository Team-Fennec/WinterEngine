using Newtonsoft.Json;
using DoomGame.Rendering;

namespace DoomGame.Resource.Types;

public struct ShaderResource
{
	public string vertexShader;
	public string fragmentShader;
}

public struct ModelResource
{
	public float[] vertices;
	public float[] colors;
	public uint[] indices;
}