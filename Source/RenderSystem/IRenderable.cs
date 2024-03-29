using Veldrid;

namespace WinterEngine.RenderSystem;

public interface IRenderable
{
    public void CreateDeviceResources(ResourceFactory factory);
    public void Render(GraphicsDevice gd, CommandList cl);
}
