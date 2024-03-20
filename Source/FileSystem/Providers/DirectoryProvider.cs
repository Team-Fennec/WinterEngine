using System.IO;
using WinterEngine.Resource;

namespace WinterEngine.Resource.Providers;

public sealed class DirectoryProvider : ResourceProvider
{
    // Logger
    private static readonly ILog log = LogManager.GetLogger(typeof(DirectoryProvider));
    
    public DirectoryProvider(string path) : base(path)
    {
        // check if the path exists
        if (!Directory.Exists(path))
        {
            log.Error($"Directory {path} does not exist!");
            throw new FileNotFoundException(); // should be a directory not found but meh
        }
    }
    
    public override StreamReader? OpenFile(string filePath)
    {
        // check if that file exists within our path
        if (!File.Exists(Path.Combine(path, filePath)))
        {
            return null;
        }
    }
}
