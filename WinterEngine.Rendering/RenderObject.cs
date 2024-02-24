using System;

namespace WinterEngine.Rendering; 
public abstract class RenderObject {
    // used to identify during debugging
    public string Name = "";

    public abstract void Render();
    public abstract void Dispose();
}
