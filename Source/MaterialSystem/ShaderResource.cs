using WinterEngine.RenderSystem;

namespace WinterEngine.Resource;

public class ShaderResource : IResource
{
    public string ShaderName { get; private set; }
    public byte[] FragmentCode { get; private set; }
    public byte[] VertexCode { get; private set; }

    public CullMode CullMode;
    public bool DepthTest;

    public void LoadData(Stream stream)
    {
        Datamodel.Datamodel shdData = Datamodel.Datamodel.Load(stream);
        stream.Close();
		
		DepthTest = shdData.Root.Get<bool>("depth_mode");
		CullMode = Enum.Parse<CullMode>(shdData.Root.Get<string>("cull_mode"));
		VertexCode = shdData.Root.Get<byte[]>("vertex_code");
		FragmentCode = shdData.Root.Get<byte[]>("fragment_code");
    }
}
