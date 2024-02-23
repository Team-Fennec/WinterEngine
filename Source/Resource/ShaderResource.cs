using Hjson;

namespace WinterEngine.Resource;

public class ShaderResource {
    public string FragmentCode;
    public string VertexCode;

    public ShaderResource(string name) {
        FileStream shaderFile = ResourceManager.OpenResource(Path.Combine("shaders", $"{name}.shd"));
        JsonValue shaderData = HjsonValue.Load(shaderFile);

        JsonValue fragVal;
        JsonValue vertVal;

        shaderData.Qo().TryGetValue("vertex", out vertVal);
        shaderData.Qo().TryGetValue("fragment", out fragVal);

        FragmentCode = fragVal.Qstr();
        VertexCode = vertVal.Qstr();
    }
}
