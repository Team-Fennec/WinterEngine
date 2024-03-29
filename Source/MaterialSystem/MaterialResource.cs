using WinterEngine.Resource;
using Veldrid;
using WinterEngine.RenderSystem;
using Vortice.Direct3D11;
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
                    ImageSharpTexture texture = new ImageSharpTexture(ResourceManager.GetData($"materials/{texPath}"));
                    prop.SetValue(this,
                        new TextureHandle(
                            texture.CreateDeviceTexture(
                                Renderer.GraphicsDevice,
                                Renderer.GraphicsDevice.ResourceFactory
                            )
                        )
                    );

                    break;
                default:
                    prop.SetValue(this, matData.Root.Get<string>(attr.PropName));
                    break;
            }
        }

        SetShaderParameters();

        m_Shader = ResourceManager.Load<ShaderResource>($"shaders/{ShaderName}.glsl");
        m_Handle = new ShaderHandle(
            ShaderName,
            m_Shader.VertexCode,
            m_Shader.FragmentCode,
            m_Shader.DepthTest,
            m_Shader.CullMode
        );
    }

    protected abstract void SetShaderParameters();
}
