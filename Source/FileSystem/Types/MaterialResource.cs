using System.Reflection;
using System.Linq;
using System.IO;
using Datamodel;

namespace WinterEngine.Resource;

[AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
public class MatPropertyAttribute : Attribute {
    public string PropName { get; private set; }

    public MatPropertyAttribute(string propName) {
        PropName = propName;
    }
}

public abstract class MaterialResource : IResource {
    const int FormatVersion = 1;
    
    public static MaterialResource Load(string matName)
    {
        StreamReader fileStream = ResourceManager.OpenResource($"materials/{matname}.wmat");
        MemoryStream mdlMem = new MemoryStream();
        fStream.BaseStream.CopyTo(mdlMem);
        fStream.Close();
        return Deserialize(mdlMem);
    }
    
    public static MaterialResource Deserialize(Stream stream)
    {
        Datamodel input = Datamodel.Load(stream);
        
        // check the shader value and try to instantiate that type
        string shaderName = input.Root.Get<string>("shader");
        var matObj = Assembly.GetExecutingAssembly().CreateInstance($"WinterEngine.Materials.{shaderName}Material");
        
        // go through it's properties and fill them out
        PropertyInfo[] props = matObj.GetType().GetProperties()
            .Where(p=>p.GetCustomAttributes(typeof(MatPropertyAttribute), true).Length != 0)
            .ToArray();
        foreach (PropertyInfo prop in props)
        {
            var attr = prop.GetCustomAttributes(typeof(MatPropertyAttribute))[0];
            prop.SetValue(matObj, input.Root.Get<string>(attr.PropName));
        }
        
        return (MaterialResource)matObj;
    }
    
    public static string Serialize(MaterialResource material)
    {
        Datamodel output = new Datamodel("material", FormatVersion);
        output.Root = new Element(output, "DmeMaterial");
        output.Root["shader"] = material.ShaderName;
        
        // run through our custom properties now
        PropertyInfo[] props = material.GetType().GetProperties()
            .Where(p=>p.GetCustomAttributes(typeof(MatPropertyAttribute), true).Length != 0)
            .ToArray();
        foreach (PropertyInfo prop in props)
        {
            var attr = prop.GetCustomAttributes(typeof(MatPropertyAttribute))[0];
            output.Root[attr.PropName] = prop.GetValue(material);
        }
        
        MemoryStream outMem = new MemoryStream();
        output.Save(outMem, "keyvalues2", 1);
        // convert stream to string
        StreamReader reader = new StreamReader(outMem);
        string text = reader.ReadToEnd();
        reader.Close();
        return text;
    }
    
    public MaterialResource()
    {
        shader = new ShaderResource(ShaderName);
    }
    
    public abstract string ShaderName { get; }
    private ShaderResource shader;
}
