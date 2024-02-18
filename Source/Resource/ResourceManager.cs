using Newtonsoft.Json;
using System.IO;
using WinterEngine.Resource.Types;
using log4net;

namespace WinterEngine.Resource;
public class ResourceManager
{
	// Logger
    private static readonly ILog log = LogManager.GetLogger(typeof(ResourceManager));

	static List<string> resDirs = new List<string>();

	public static void AddResourceDirectory(string path) {
		if (Directory.Exists(path)) {
			log.Info($"Added resource path {path}");
			resDirs.Add(path);
		} else {
			log.Error($"Failed to add resource directory: No directory was found at path {path}.");
		}
	}

	public static FileStream OpenResource(string path) {
		foreach (string resDir in resDirs) {
			if (File.Exists(Path.Combine(resDir, path))) {
				return File.OpenRead(Path.Combine(resDir, path));
			}
		}

		FileNotFoundException except = new FileNotFoundException();
		log.Fatal($"Cannot find a resource by path {path}", except);
		throw except;
	}
	public static string GetResContents(string path) {
		foreach (string resDir in resDirs) {
			if (File.Exists(Path.Combine(resDir, path))) {
				return File.ReadAllText(Path.Combine(resDir, path));
			}
		}

		FileNotFoundException except = new FileNotFoundException();
		log.Fatal($"Cannot find a resource by path {path}", except);
		throw except;
	}
}