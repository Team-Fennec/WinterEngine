using WinterEngine.Rendering.RenderObjects;
using WinterEngine.Rendering;
using WinterEngine.Resource;

namespace WinterEngine.Interfaces;

public interface IHasModel {
    public ROModel RenderObject { get; set; }
    public ShaderHandle Shader { get; set; }
    public MaterialResource Material { get; set; }
}
