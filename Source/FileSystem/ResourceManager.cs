using System.IO;
using WinterEngine.Core;

namespace WinterEngine.Resource;

public interface IResource
{
    public void ReadData(Stream data);
}

public abstract class ResourceProvider
{
    string path;
    
    public ResourceProvider(string path)
    {
        this.path = path;
    }
    
	public abstract Stream OpenFile(string filePath);
	public abstract bool FileExists(string filePath);
	public abstract bool DirExists(string dirPath);
}

public class ResourceManager
{
	// Logger
    private static readonly ILog log = LogManager.GetLogger(typeof(ResourceManager));

	static List<ResourceProvider> resDirs = new List<ResourceProvider>();
	static Dictionary<Type, IResource> registeredResources = new Dictionary<Type, IResource>();
	
	public static void AddProvider(ResourceProvider resProvide)
	{
	    resDirs.Add(resProvide);
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
    	    T retRes = new T();
    	    t.LoadData()
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