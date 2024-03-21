using System;
using MathLib;
using WinterEngine.RenderSystem;

namespace WinterEngine.Resource;

public abstract class ModelResource : IResource
{
    public List<MaterialResource> Materials;
    public abstract (Vertex[], ushort[]) GetData();

    private MeshHandle handle;
}
