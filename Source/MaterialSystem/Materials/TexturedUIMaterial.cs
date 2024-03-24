using Veldrid;
using WinterEngine.MaterialSystem;
using WinterEngine.RenderSystem;

namespace WinterEngine.Materials;

public class TexturedUIMaterial : MaterialResource
{
    public override string ShaderName => "TexturedUI";

    [MatProperty("basetexture", ShaderParamType.Texture2D)]
    public TextureHandle SamplerTexture { get; set; }

    protected override void SetShaderParameters()
    {
        m_Handle.SetParameter("SamplerTexture", ShaderParamType.Texture2D, SamplerTexture.TextureView, ShaderStages.Fragment);
    }
}
