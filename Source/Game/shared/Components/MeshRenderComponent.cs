using WinterEngine.MaterialSystem;
using WinterEngine.RenderSystem;
using WinterEngine.SceneSystem;

using gMaterialSystem = WinterEngine.MaterialSystem.MaterialSystem;

namespace WinterEngine.Components;

public class MeshRenderComponent : EntityComponent
{
    private TexturedMeshRO m_RenderModel;

    public override void Awake()
    {
        // load md3 resource, grab material
    }

    public override void OnEnable()
    {
        Renderer.AddRenderObject(m_RenderModel);
    }
}
