namespace WinterEngine.Resource;

public interface IResource
{
    public void LoadData(Stream stream);
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

    public static void AddProvider(ResourceProvider resProvide)
    {
        resDirs.Add(resProvide);
    }

    public static Stream GetData(string path)
    {
        foreach (ResourceProvider resProv in resDirs)
        {
            if (resProv.FileExists(path))
            {
                return resProv.OpenFile(path);
            }
        }

        FileNotFoundException except = new FileNotFoundException();
        log.Error($"Cannot find a resource by path {path}");
        throw except;
    }

    public static IResource Load(Type type, string path)
    {
        if (!type.IsAssignableTo(typeof(IResource)))
            throw new ArgumentException($"Invalid type ({type.Name}) provided, type must be subclass of IResource.");

        Stream resData;

        foreach (ResourceProvider resProv in resDirs)
        {
            if (resProv.FileExists(path))
            {
                resData = resProv.OpenFile(path);

                var instObj = Activator.CreateInstance(type);
                if (instObj == null)
                    throw new Exception($"Unknown error occurred trying to load material of type {type.Name}!");
                IResource retRes = (IResource)instObj;
                retRes.LoadData(resData);
                return retRes;
            }
        }

        FileNotFoundException except = new FileNotFoundException();
        log.Error($"Cannot find a resource by path {path}");
        throw except;
    }

    public static T Load<T>(string path) where T : IResource, new()
    {
        Stream resData;

        foreach (ResourceProvider resProv in resDirs)
        {
            if (resProv.FileExists(path))
            {
                resData = resProv.OpenFile(path);

                T retRes = new T();
                retRes.LoadData(resData);
                return retRes;
            }
        }

        FileNotFoundException except = new FileNotFoundException();
        log.Error($"Cannot find a resource by path {path}");
        throw except;
    }
}