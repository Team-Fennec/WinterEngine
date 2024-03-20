using System.Text;
using Veldrid;
using Veldrid.SPIRV;

namespace WinterEngine.Rendering;
public class ShaderHandle {
    private static readonly ILog log = LogManager.GetLogger("RenderSystem");

    public string ShaderName { get; private set; }
    public Shader VertexShader { get; private set; }
    public Shader FragmentShader { get; private set; }

    public ShaderHandle(string shaderName, string vtxCode, string frgCode) {
        ShaderName = shaderName;

        ShaderDescription vertexShaderDesc = new ShaderDescription(
            ShaderStages.Vertex,
            Encoding.UTF8.GetBytes(vtxCode),
            "main");
        ShaderDescription fragmentShaderDesc = new ShaderDescription(
            ShaderStages.Fragment,
            Encoding.UTF8.GetBytes(frgCode),
            "main");

        Shader[] shaders = Renderer.GraphicsDevice.ResourceFactory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

        VertexShader = shaders[0];
        FragmentShader = shaders[1];
    }

    ~ShaderHandle() {
        if (!VertexShader.IsDisposed || !FragmentShader.IsDisposed) {
            // we could handle this ourselves but that's not encouraging good practices.
            // scream at the programmer instead.
            log.Warn("Resources Leaked! Make sure you're calling Dispose before deconstruction!");
        }
    }

    public void Dispose() {
        VertexShader.Dispose();
        FragmentShader.Dispose();
    }
}
