using System.IO;
using System.IO.Compression;
using log4net;
using WinterEngine.Core;

namespace WinterEngine.Resource;

public enum ResourceFormat {
	Snowpack,
	Folder
}

public struct ResourceProvider {
	public ResourceFormat format;
	public string path;

	public ResourceProvider(string _path, ResourceFormat _format) {
		format = _format;
		path = _path;
	}
}

public class ResourceManager
{
	// Logger
    private static readonly ILog log = LogManager.GetLogger(typeof(ResourceManager));

	static List<ResourceProvider> resDirs = new List<ResourceProvider>();
	
	public static void AddResourceProvider(string path, ResourceFormat format = ResourceFormat.Folder) {
		switch(format) {
			case ResourceFormat.Folder:
				if (Directory.Exists(path)) {
					log.Info($"Added resource path {path}");
					resDirs.Add(new ResourceProvider(path, format));
				} else {
					throw new FileNotFoundException($"Failed to add resource directory: No directory was found at path {path}.");
				}
				break;
			case ResourceFormat.Snowpack:
				throw new NotImplementedException();
		}
	}

	public static StreamReader OpenResource(string path) {
		foreach (ResourceProvider resDir in resDirs) {
			switch(resDir.format) {
			case ResourceFormat.Folder:
				if (File.Exists(Path.Combine(resDir.path, path))) {
					return new StreamReader(Path.Combine(resDir.path, path));
				}
				break;
			case ResourceFormat.Snowpack:
				throw new NotImplementedException();
			}
		}

		FileNotFoundException except = new FileNotFoundException();
		log.Fatal($"Cannot find a resource by path {path}", except);
		throw except;
	}
}