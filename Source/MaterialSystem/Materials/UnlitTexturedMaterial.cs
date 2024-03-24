using WinterEngine.MaterialSystem;
using WinterEngine.RenderSystem;
using Veldrid;

namespace WinterEngine.Materials;

public class UnlitTexturedMaterial : MaterialResource
{
    public override string ShaderName => "UnlitTextured";

    [MatProperty("basetexture", ShaderParamType.Texture2D)]
    public TextureHandle SamplerTexture { get; set; }

    protected override void SetShaderParameters()
    {
        m_Handle.SetParameter("SamplerTexture", ShaderParamType.Texture2D, SamplerTexture.TextureView, ShaderStages.Fragment);
    }
}
