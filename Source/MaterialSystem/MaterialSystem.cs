using Datamodel;
using log4net;
using System.Reflection;
using WinterEngine.Resource;

namespace WinterEngine.MaterialSystem;

public static class MaterialSystem
{
    const int FormatVersion = 1;

    // jank as fuck but until I figure out the IResource nonsense it's the best we got.
    public static MaterialResource Load(string matName)
    {
        Stream stream;
        try
        {
            stream = ResourceManager.GetData($"materials/{matName}.wmat");
        }
        catch
        {
            LogManager.GetLogger("MaterialSystem").Error($"Unable to load material {matName}");
            stream = ResourceManager.GetData($"materials/engine/missing.wmat");
        }
        Datamodel.Datamodel input = Datamodel.Datamodel.Load(stream);

        // check the shader value and try to instantiate that type
        string shaderName = input.Root.Get<string>("shader");
        var matType = Assembly.GetExecutingAssembly().GetType($"WinterEngine.Materials.{shaderName}Material");
        if (matType == null)
        {
            throw new ArgumentException($"No material of type {shaderName} exists.");
        }

        input.Dispose();

        MaterialResource matRes = Assembly.GetExecutingAssembly().CreateInstance(matType.FullName) as MaterialResource;
        if (matRes == null)
        {
            throw new Exception("Failed to create material resource!");
        }
        matRes.LoadData(stream);

        return matRes;
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
