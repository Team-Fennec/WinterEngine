using System.IO;

namespace WinterEngine.Resource.Types;

// Holds data about a model
public class ModelResource
{
	public float[] vertices;
	public float[] colors;
	public uint[] indices;
	public float[] texCoords;

	public string name;
	public string material;

	public ModelResource(string fileName) {
	}
}
