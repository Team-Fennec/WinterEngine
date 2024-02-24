using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using log4net;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
					Engine.Error($"Failed to add resource directory: No directory was found at path {path}.");
					return;
				}
				break;
			case ResourceFormat.Snowpack:
				/*if (File.Exists(path)) {
					FileStream file = File.OpenRead(path);
					byte[] _buf = new byte[sizeof(int)];
					file.Read(_buf, 0, sizeof(int));
					if (BitConverter.ToInt32(_buf) != SnowPack.ident) {
						log.Error($"Failed to add resource pack: Not snowpack format.");
						return;
					}
					file.Read(_buf, 0, sizeof(int));
					if (BitConverter.ToInt32(_buf) > SnowPack.version) {
						log.Error($"Failed to add resource pack: Version is too new");
						return;
					}
					file.Close();
					resDirs.Add(new ResourceProvider(path, format));
					log.Info($"Added resource pack {path}");
				}*/
				break;
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
				/*SnowPack packData = new SnowPack();
				FileStream packStream = File.OpenRead(resDir.path);
				byte[] _buf = new byte[sizeof(int)];
				packStream.Read(_buf, 0, sizeof(int));

				if (BitConverter.ToInt32(_buf, 0) != SnowPack.Ident) {
					Console.WriteLine("Invalid file, not an SPK file");
					throw new Exception();
				}

				packStream.Read(_buf, 0, sizeof(int));

				if (BitConverter.ToInt32(_buf, 0) > SnowPack.Version) {
					Console.WriteLine("Invalid file, version is too new");
					throw new Exception();
				}
				packData.zipData = new byte[packStream.Length - packStream.Position];

				packStream.Read(packData.zipData, 0, (int)(packStream.Length - packStream.Position));
				packStream.Close();
				return new StreamReader(new ZipArchive(new MemoryStream(packData.zipData)).GetEntry(path).Open());*/
				throw new Exception("Not implemented");
			}
		}

		FileNotFoundException except = new FileNotFoundException();
		log.Fatal($"Cannot find a resource by path {path}", except);
		throw except;
	}
}