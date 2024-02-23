using Newtonsoft.Json;

namespace WinterEngine.Resource.Types;

public struct ModelResource
{
	public float[] vertices;
	public float[] colors;
	public uint[] indices;
	public float[] texCoords;
}