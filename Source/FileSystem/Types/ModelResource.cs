using System;
using WinterEngine.Rendering;

namespace WinterEngine.Resource;

public abstract class ModelResource : IResource
{
    public List<MaterialResource> Materials;
    public abstract (VertexPositionColorTexture[], ushort[]) GetData();
    
    // todo(model resource): should this hold a render function to render it's data or?
}
