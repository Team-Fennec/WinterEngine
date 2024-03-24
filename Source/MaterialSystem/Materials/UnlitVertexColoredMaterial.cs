using WinterEngine.MaterialSystem;

namespace WinterEngine.Materials;

public class UnlitVertexColoredMaterial : MaterialResource
{
    public override string ShaderName => "UnlitVertexColored";

    protected override void SetShaderParameters()
    {
        // nothing needed
    }
}
