using System.IO;

namespace WinterEngine.Resource;

public interface IResource
{
    public ResourceProvider(Stream stream) { }
}

public abstract class ResourceProvider
{
    public abstract Stream OpenFile(string filePath);
    public abstract bool FileExists(string filePath);
    public abstract bool DirExists(string dirPath);
}

public class ResourceManager
{
    // Logger
    private static readonly ILog log = LogManager.GetLogger("ResourceManager");

    static List<ResourceProvider> resDirs = new List<ResourceProvider>();
    static Dictionary<Type, IResource> registeredResources = new Dictionary<Type, IResource>();

    public static void AddProvider(ResourceProvider resProvide)
    {
        resDirs.Add(resProvide);
    }

    public static Stream GetData(string path)
    {
        Stream resData;

        foreach (ResourceProvider resProv in resDirs)
        {
            if (resProv.FileExists(path))
            {
                resData = resProv.OpenFile(path);
                break;
            }
        }

        if (resData != null)
        {
            return resData;
        }
        else
        {
            FileNotFoundException except = new FileNotFoundException();
            log.Error($"Cannot find a resource by path {path}");
            throw except;
        }
    }

    public static T Load<T>(string path) where T : IResource, new()
    {
        Stream resData;

        foreach (ResourceProvider resProv in resDirs)
        {
            if (resProv.FileExists(path))
            {
                resData = resProv.OpenFile(path);
                break;
            }
        }

        if (resData != null)
        {
            T retRes = new T(resData);
            return retRes;
        }
        else
        {
            FileNotFoundException except = new FileNotFoundException();
            log.Error($"Cannot find a resource by path {path}");
            throw except;
        }
    }
}