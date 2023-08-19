using Newtonsoft.Json;
using System.IO;
using DoomGame.Debug;
using DoomGame.Resource.Types;

namespace DoomGame.Resource;

// TODO: give this a reason to exist other than to house this function
public class ResourceLoader
{
	public static ModelResource LoadModel(string modelName)
	{
		if (!File.Exists($"game_data/models/{modelName}"))
		{
			Logger.Log("Resource", $"Could not find model file: \"{modelName}\"", LogType.Error);
		}

		string modelSource = File.ReadAllText($"game_data/models/{modelName}");
		ModelResource modelData = JsonConvert.DeserializeObject<ModelResource>(modelSource);

		Logger.Log("Resource", $"Loaded model data \"{modelName}\"", LogType.Info);

		return modelData;
	}
}