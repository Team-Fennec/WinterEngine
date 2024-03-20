using WinterEngine.Resource;

namespace WinterEngine.Materials;

public class UnlitTexturedMaterial : MaterialResource {
    public override string ShaderName => "UnlitTextured";

    [MatProperty("basetexture")]
    public string BaseTexture { get; set; }
}
