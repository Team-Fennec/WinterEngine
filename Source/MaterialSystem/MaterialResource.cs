using WinterEngine.Resource;
using WinterEngine.RenderSystem;
using System.Reflection;
using Veldrid.ImageSharp;

namespace WinterEngine.MaterialSystem;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class MatPropertyAttribute : Attribute
{
    public string PropName { get; private set; }
    public ShaderParamType PropType { get; private set; }

    public MatPropertyAttribute(string propName, ShaderParamType propType)
    {
        PropName = propName;
        PropType = propType;
    }
}

public abstract class MaterialResource : IResource
{
    public abstract string ShaderName { get; }
    public ShaderHandle GetHandle() => m_Handle;
    protected ShaderResource m_Shader;
    protected ShaderHandle m_Handle;

    public void LoadData(Stream stream)
    {
        Datamodel.Datamodel matData = Datamodel.Datamodel.Load(stream);
        stream.Close();

        // go through it's properties and fill them out
        PropertyInfo[] props = GetType().GetProperties()
            .Where(p=>p.GetCustomAttributes(typeof(MatPropertyAttribute), true).Length != 0)
            .ToArray();
        foreach (PropertyInfo prop in props)
        {
            var attr = (MatPropertyAttribute)prop.GetCustomAttributes(typeof(MatPropertyAttribute)).ToArray()[0];

            // attr data
            switch (attr.PropType)
            {
                case ShaderParamType.Int:
                    prop.SetValue(this, matData.Root.Get<int>(attr.PropName));
                    break;
                case ShaderParamType.Float:
                    prop.SetValue(this, matData.Root.Get<float>(attr.PropName));
                    break;
                case ShaderParamType.Texture2D:
                    string texPath = matData.Root.Get<string>(attr.PropName);

                    // load texture from resources
                    Stream data = ResourceManager.GetData($"materials/{texPath}");
                    ImageSharpTexture texture = new ImageSharpTexture(data);
                    prop.SetValue(this,
                        new TextureHandle(
                            texture.CreateDeviceTexture(
                                Renderer.GraphicsDevice,
                                Renderer.GraphicsDevice.ResourceFactory
                            )
                        )
                    );
                    data.Close();

                    break;
                default:
                    prop.SetValue(this, matData.Root.Get<string>(attr.PropName));
                    break;
            }
        }

        m_Shader = ResourceManager.Load<ShaderResource>($"shaders/{ShaderName}.shd");
        m_Handle = new ShaderHandle(
            ShaderName,
            m_Shader.VertexCode,
            m_Shader.FragmentCode,
            m_Shader.DepthTest,
            m_Shader.CullMode
        );
        m_Handle.VertexShader.Name = $"{ShaderName}_VertexShader";
        m_Handle.FragmentShader.Name = $"{ShaderName}_FragmentShader";

        SetShaderParameters();
    }

    protected abstract void SetShaderParameters();
}
