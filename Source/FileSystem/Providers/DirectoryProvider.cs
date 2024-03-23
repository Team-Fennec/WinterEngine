using System.IO;
using WinterEngine.Resource;

namespace WinterEngine.Resource.Providers;

public sealed class DirectoryProvider : ResourceProvider
{
    // Logger
    private static readonly ILog log = LogManager.GetLogger(typeof(DirectoryProvider));
    string path;

    public DirectoryProvider(string path)
    {
        // check if the path exists
        if (!Directory.Exists(path))
        {
            log.Error($"Directory {path} does not exist!");
            throw new FileNotFoundException(); // should be a directory not found but meh
        }

        this.path = path;
    }

    public override Stream? OpenFile(string filePath)
    {
        // check if that file exists within our path
        if (!File.Exists(Path.Combine(path, filePath)))
        {
            return null;
        }

        return File.Open(Path.Combine(path, filePath));
    }
}
