using System.Numerics;

namespace WinterEngine.Rendering.RenderObjects;

public class ROModel : RenderObject {
    public TextureHandle Texture;
    public ShaderHandle Shader;
    public MeshHandle Mesh;

    // todo: add support for defining specific positions and shit to rendering
    public Vector3 Position;
    public Vector3 Rotation;

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
