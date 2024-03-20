using WinterEngine.Resource;

namespace WinterEngine.Materials;

public class TexturedUIMaterial : MaterialResource {
    public override string ShaderName => "TexturedUI";

    [MatProperty("basetexture")]
    public string BaseTexture { get; set; }
}
