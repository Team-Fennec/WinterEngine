using System.IO;
using Microsoft.Xna.Framework;

namespace WinterEngine.Resource.Types;

public struct ModelFace {
    public int vert1;
    public int vert2;
    public int vert3;

	public int norm1;
    public int norm2;
    public int norm3;

	public int uv1;
    public int uv2;
    public int uv3;
}

public struct ModelObject {
	public ModelObject() {
		name = "";
		faces = new List<ModelFace>();
	}

	public string name;
    public List<ModelFace> faces;
}

public struct ModelFrame {
	public ModelFrame() {
		objects = new List<ModelObject>();
		vertices = new List<Vector3>();
		normals = new List<Vector3>();
		texCoords = new List<Vector2>();
	}

    public List<ModelObject> objects;
	public List<Vector3> vertices;
    public List<Vector3> normals;
    public List<Vector2> texCoords;
}

// Holds data about a model
public class ModelResource
{
	public string name = "Unused"; // unused
	public string material;
	public List<ModelFrame> frames;

	public ModelResource(string modelPath) {
		StreamReader modelFile = ResourceManager.OpenResource($"models/{modelPath}.wmdl");

		frames = new List<ModelFrame>();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
		while (true) {
			string line = modelFile.ReadLine();
			if (line == null) {
				break;
			}

            if (line.StartsWith("#")) {
				continue; // skip over comments
			}

            string[] splitLine = line.Split(" ");

			if (splitLine[0] == "tex") {
				material = splitLine[1];
				continue;
			}

			if (splitLine[0] == "framestart") {
				ModelFrame frame = new ModelFrame();
				while (line != "frameend") {
					line = modelFile.ReadLine();
					splitLine = line.Split(" ");
					if (splitLine[0] == "o") {
						ModelObject obj = new ModelObject();
						obj.name = splitLine[1];
						while (line != "endo") {
							line = modelFile.ReadLine();
							splitLine = line.Split(" ");

							switch (splitLine[0]) {
								case "v":
									frame.vertices.Add(new Vector3(
										float.Parse(splitLine[1]), 
										float.Parse(splitLine[2]),
										float.Parse(splitLine[3])
									));
									break;
								case "vn":
									frame.normals.Add(new Vector3(
										float.Parse(splitLine[1]), 
										float.Parse(splitLine[2]),
										float.Parse(splitLine[3])
									));
									break;
								case "vt":
									frame.texCoords.Add(new Vector2(
										float.Parse(splitLine[1]), 
										float.Parse(splitLine[2])
									));
									break;
								case "f":
									ModelFace face = new ModelFace();
									string[] v1 = splitLine[1].Split("/");
									string[] v2 = splitLine[2].Split("/");
									string[] v3 = splitLine[3].Split("/");

									face.vert1	= int.Parse(v1[0]) - 1;
									face.uv1	= int.Parse(v1[1]) - 1;
									face.norm1	= int.Parse(v1[2]) - 1;
									
									face.vert2	= int.Parse(v2[0]) - 1;
									face.uv2	= int.Parse(v2[1]) - 1;
									face.norm2	= int.Parse(v2[2]) - 1;

									face.vert3	= int.Parse(v3[0]) - 1;
									face.uv3	= int.Parse(v3[1]) - 1;
									face.norm3	= int.Parse(v3[2]) - 1;

									obj.faces.Add(face);
									break;
							}
						}
						frame.objects.Add(obj);
					}
				}
				frames.Add(frame);
			}
		}
		modelFile.Close();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
	}
}