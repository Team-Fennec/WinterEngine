using System.Text;
using Veldrid;
using Veldrid.SPIRV;
using log4net;

namespace WinterEngine.RenderSystem;

public enum ShaderParamType
{
    Int,
    Float,
    Texture2D,
    Sampler,
    Vector2,
    Vector3,
    Vector4,
}

public struct ShaderParam
{
    public string Name;
    public ResourceKind Kind;
    public BindableResource Value;
    public ShaderStages Stage;
}

public class ShaderHandle {
    private static readonly ILog log = LogManager.GetLogger("RenderSystem");

    public string ShaderName { get; private set; }
    public Shader VertexShader { get; private set; }
    public Shader FragmentShader { get; private set; }
    public FaceCullMode CullMode { get; private set; }
    public bool DepthTest { get; private set; }
    public List<ShaderParam> Params = new List<ShaderParam>();

    public ShaderHandle(string shaderName, byte[] vtxCode, byte[] frgCode, bool depthTest, CullMode cullMode) {
        ShaderName = shaderName;

        Shader[] shaders = Renderer.GraphicsDevice.ResourceFactory.CreateFromSpirv(
            new ShaderDescription(ShaderStages.Vertex, vtxCode, "main"),
            new ShaderDescription(ShaderStages.Fragment, frgCode, "main")
        );

        VertexShader = shaders[0];
        FragmentShader = shaders[1];

        DepthTest = depthTest;
        switch (cullMode)
        {
            case RenderSystem.CullMode.Back:
                CullMode = FaceCullMode.Back;
                break;
            case RenderSystem.CullMode.Front:
                CullMode = FaceCullMode.Front;
                break;
            case RenderSystem.CullMode.None:
                CullMode = FaceCullMode.None;
                break;
        }
    }

    ~ShaderHandle() {
        if (!VertexShader.IsDisposed || !FragmentShader.IsDisposed) {
            // we could handle this ourselves but that's not encouraging good practices.
            // scream at the programmer instead.
            log.Warn("Resources Leaked! Make sure you're calling Dispose before deconstruction!");
        }
    }

    public void SetParameter(string Name, ShaderParamType Type, BindableResource Value, ShaderStages Stage)
    {
        ResourceKind Kind = ResourceKind.UniformBuffer;
        switch (Type)
        {
            case ShaderParamType.Int:
            case ShaderParamType.Float:
            case ShaderParamType.Vector2:
            case ShaderParamType.Vector3:
            case ShaderParamType.Vector4:
                Kind = ResourceKind.UniformBuffer;
                break;
            case ShaderParamType.Texture2D:
                Kind = ResourceKind.TextureReadOnly;
                break;
            case ShaderParamType.Sampler:
                Kind = ResourceKind.Sampler;
                break;
        }

        for (int i = 0; i < Params.Count; i++)
        {
            if (Params[i].Name == Name)
            {
                Params.RemoveAt(i);
                break;
            }
        }

        Params.Add(new ShaderParam()
        {
            Name = Name,
            Kind = Kind,
            Value = Value,
            Stage = Stage
        });
    }

    public void Dispose() {
        VertexShader.Dispose();
        FragmentShader.Dispose();
    }
}
