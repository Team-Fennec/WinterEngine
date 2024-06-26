using Veldrid;
using log4net;
using MathLib;

namespace WinterEngine.RenderSystem;

public class MeshHandle
{
    private static readonly ILog log = LogManager.GetLogger("RenderSystem");

    public DeviceBuffer VertexBuffer { get; private set; }
    public DeviceBuffer IndexBuffer { get; private set; }

    public Vertex[] Vertices;
    public uint[] Indices;

    public uint IndexCount => (uint)Indices.Length;

    public MeshHandle(Vertex[] vertices, uint[] indices)
    {
        Vertices = vertices;
        Indices = indices;

        VertexBuffer = Renderer.GraphicsDevice.ResourceFactory.CreateBuffer(
            new BufferDescription(
                (uint)(Vertex.SizeInBytes * Vertices.Length),
                BufferUsage.VertexBuffer
            )
        );
        IndexBuffer = Renderer.GraphicsDevice.ResourceFactory.CreateBuffer(
            new BufferDescription(
                sizeof(uint) * (uint)Indices.Length,
                BufferUsage.IndexBuffer
            )
        );

        Renderer.GraphicsDevice.UpdateBuffer(VertexBuffer, 0, Vertices);
        Renderer.GraphicsDevice.UpdateBuffer(IndexBuffer, 0, Indices);
    }

    ~MeshHandle()
    {
        if (!VertexBuffer.IsDisposed || !IndexBuffer.IsDisposed)
        {
            // don't run it ourselves, encourage developers to manually dispose for best practices
            // that way we always know when things are getting disposed of.
            log.Warn("Resources Leaked! Make sure you're calling Dispose before deconstruction!");
        }
    }

    public void Update(Vertex[] vertices, uint[] indices)
    {
        // This doesn't check or change sizes because if you want a different model
        // then you should be making a new handle. This function is meant for updating
        // existing vertices for vertex based animation.
        if (vertices.Length != Vertices.Length || indices.Length != Indices.Length)
        {
            throw new ArgumentException("Do not use Update to swap entire meshes! Update is meant for same mesh adjustment ONLY!");
        }

        Vertices = vertices;
        Indices = indices;

        Renderer.GraphicsDevice.UpdateBuffer(VertexBuffer, 0, Vertices);
        Renderer.GraphicsDevice.UpdateBuffer(IndexBuffer, 0, Indices);
    }

    public void Dispose()
    {
        VertexBuffer.Dispose();
        IndexBuffer.Dispose();
    }
}
