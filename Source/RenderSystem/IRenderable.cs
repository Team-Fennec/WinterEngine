using Veldrid;
using Veldrid.Utilities;

namespace WinterEngine.RenderSystem;

public interface IRenderable
{
    /// <summary>
    /// DO NOT USE THIS OUTSIDE OF THE HOLDER ENTITY.
    /// </summary>
    public DisposeCollectorResourceFactory m_Factory { get; set; }

    public void CreateDeviceResources();
    public void Render(GraphicsDevice gd, CommandList cl);
}
