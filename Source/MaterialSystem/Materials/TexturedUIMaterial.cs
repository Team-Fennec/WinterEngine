using WinterEngine.MaterialSystem;
using WinterEngine.RenderSystem;

namespace WinterEngine.Materials;

public class TexturedUIMaterial : MaterialResource
{
    public override string ShaderName => "TexturedUI";

    [MatProperty("basetexture", MatPropType.Texture2D)]
    public TextureHandle SamplerTexture { get; set; }
}
