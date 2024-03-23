using Veldrid;
using System.Numerics;
using MathLib;

namespace WinterEngine.RenderSystem;

public abstract class RenderObject
{
    public string ID => ($"{Name}_{Guid}");
    public string Name;
    public readonly Guid Guid = Guid.NewGuid();
    public bool IsDisposed => m_IsDisposed;
    private bool m_IsDisposed = false;

    public abstract void Render(GraphicsDevice gd, CommandList cl);
    public abstract void CreateDeviceResources();
    public abstract void DisposeResources();
}
