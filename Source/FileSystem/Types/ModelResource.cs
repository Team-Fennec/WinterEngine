using System;
using MathLib;

namespace WinterEngine.Resource;

public abstract class ModelResource : IResource
{
    public List<MaterialResource> Materials;
    public abstract (Vertex[], ushort[]) GetData();
}
