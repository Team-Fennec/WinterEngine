using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterEngine.Rendering.RenderObjects; 
public class ROModel : RenderObject {
    public TextureHandle Texture;
    public ShaderHandle Shader;
    public MeshHandle Mesh;

    public ROModel() {
        Name = $"RO_Model_{Guid.NewGuid()}";
    }

    public override void Dispose() {
        Texture.Dispose();
        Shader.Dispose();
        Mesh.Dispose();
    }

    public override void Render() {
        Renderer.UseShader(Shader);
        Renderer.UseTexture(Texture);
        Renderer.DrawMesh(Mesh);
    }
}
