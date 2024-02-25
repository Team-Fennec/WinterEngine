using WinterEngine.Resource;

namespace WinterEngine.Materials;

public class UnlitTexturedMaterial : MaterialResource {
    public override string ShaderName => "UnlitTextured";

    [MatProperty(typeof(string), "albedo")]
    public string AlbedoTexture { get; set; }
}
