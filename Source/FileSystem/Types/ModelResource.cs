using System;
using MathLib;
using WinterEngine.RenderSystem;

namespace WinterEngine.Resource;

public abstract class MeshResource : IResource
{
    public abstract (Vertex[], ushort[]) GetData();

    private MeshHandle m_Handle;
}
