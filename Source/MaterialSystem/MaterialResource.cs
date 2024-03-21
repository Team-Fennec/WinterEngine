using System.Reflection;
using System.Linq;
using System.IO;
using Datamodel;
using WinterEngine.Resource;

namespace WinterEngine.MaterialSystem;

[AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
public class MatPropertyAttribute : Attribute {
    public string PropName { get; private set; }

    public MatPropertyAttribute(string propName) {
        PropName = propName;
    }
}

public class MaterialResource {
    const int FormatVersion = 1;
    
    public static MaterialResource Load(string matName)
    {
        Stream fileStream = ResourceManager.GetData($"materials/{matname}.wmat");
        return Deserialize(fileStream);
    }
    
    public static MaterialResource Deserialize(Stream stream)
    {
        Datamodel input = Datamodel.Load(stream);
        stream.Close();
        
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
