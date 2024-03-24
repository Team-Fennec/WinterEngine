using Datamodel;
using System.Reflection;
using WinterEngine.Resource;

namespace WinterEngine.MaterialSystem;

public static class MaterialSystem
{
    const int FormatVersion = 1;

    public static MaterialResource Load(string matName)
    {
        Stream stream = ResourceManager.GetData($"materials/{matName}.wmat");

        Datamodel.Datamodel input = Datamodel.Datamodel.Load(stream);
        stream.Close();

        // check the shader value and try to instantiate that type
        string shaderName = input.Root.Get<string>("shader");
        var matType = Assembly.GetExecutingAssembly().GetType($"WinterEngine.Materials.{shaderName}Material");
        if (matType == null)
        {
            throw new ArgumentException($"No material of type {shaderName} exists.");
        }

        return (MaterialResource)ResourceManager.Load(matType, $"materials/{matName}.wmat");
    }

    public static string Serialize(MaterialResource material)
    {
        Datamodel.Datamodel output = new Datamodel.Datamodel("material", FormatVersion);
        output.Root = new Element(output, "DmeMaterial");
        output.Root["shader"] = material.ShaderName;

        // run through our custom properties now
        PropertyInfo[] props = material.GetType().GetProperties()
            .Where(p=>p.GetCustomAttributes(typeof(MatPropertyAttribute), true).Length != 0)
            .ToArray();
        foreach (PropertyInfo prop in props)
        {
            var attr = (MatPropertyAttribute)prop.GetCustomAttributes(typeof(MatPropertyAttribute)).ToArray()[0];
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
}
