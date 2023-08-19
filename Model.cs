using DoomGame.Resource.Types;
using DoomGame.Resource;
using DoomGame.Rendering;
using DoomGame.Debug;

namespace DoomGame.Objects;

// Takes a model resource and builds the proper data sets
// to use the model.
public class Model
{
	private ModelResource modelResource;

	public float[] Vertices { get => vertices; }
	public uint[] Indices { get => indices; }

	private float[] vertices;
	private uint[] indices;

	public Model()
	{
		modelResource = new();
	}

	public Model(ModelResource model)
	{
		modelResource = model;

		CompileModel();
	}

	public Model(float[] _vertices, float[] _colors, uint[] _indices)
	{
		modelResource = new()
		{
			vertices = _vertices,
			colors = _colors,
			indices = _indices
		};

		CompileModel();
	}

	// takes the current resource data and builds the proper
	// opengl compatible vertex list
	public void CompileModel()
	{
		List<float> fullArray = new();

		for (int i = 0; i < modelResource.vertices.Length; i += 3)
		{
			fullArray.Add(modelResource.vertices[i]);
			fullArray.Add(modelResource.vertices[i+1]);
			fullArray.Add(modelResource.vertices[i+2]);

			fullArray.Add(modelResource.colors[i]);
			fullArray.Add(modelResource.colors[i+1]);
			fullArray.Add(modelResource.colors[i+2]);
		}

		vertices = fullArray.ToArray();

		indices = modelResource.indices;

		Logger.Log("Model", "Compiled model data.", LogType.Info);
	}
}