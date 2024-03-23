using Veldrid;
using Veldrid.Utilities;
using System.Numerics;
using MathLib;

namespace WinterEngine.RenderSystem;

public class TexturedMeshRO : RenderObject
{
    // we hold onto our handle objects since some can be updated
    private ShaderHandle m_Shader;
    private MeshHandle m_Mesh;
    private TextureHandle m_Texture;
    
    private Vector3 m_LocalOrigin;
    private Vector3 m_Position;
    private Vector3 m_Rotation;
    private Vector3 m_Scale;
    
    private Pipeline m_Pipeline;
    private DeviceBuffer m_LocalWorldBuffer;
    private ResourceSet m_TextureSet;
    private ResourceSet m_ProjViewSet;
    
    private readonly DisposeCollector m_DisposeCollector = new DisposeCollector();
    
    public class RenderObject(string name, ShaderHandle Shader, MeshHandle Mesh, TextureHandle Texture)
    {
        Name = name;
        // Assign our data
        m_Shader = Shader;
        m_Mesh = Mesh;
        m_Texture = Texture;
        
        m_LocalOrigin = Vector3.Zero;
        m_Position = Vector3.Zero;
        m_Rotation = Vector3.Zero;
        m_Scale = Vector3.One;
        
        // call to create resources
        CreateDeviceResources();
    }
    
    // fuck quaternions they make my brain hrut :(
    public void SetTransform(Vector3 Position, Vector3 Rotation, Vector3 Scale)
    {
        m_Position = Position;
        m_Rotation = Rotation;
        m_Scale = Scale;
    }
    
    public void SetOrigin(Vector3 Origin)
    {
        m_LocalOrigin = Origin;
    }
    
    public override void DisposeResources()
    {
        m_DisposeCollector.DisposeAll();
        m_IsDisposed = true;
    }
    
    public override void CreateDeviceResources()
    {
        ResourceFactory factory = new DisposeCollectorResourceFactory(Renderer.GraphicsDevice.ResourceFactory, _disposeCollector);
        
        #region Create Pipeline   
        pipelineDescription = new GraphicsPipelineDescription();
        pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

        pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
            depthTestEnabled: m_Shader.DepthTest,
            depthWriteEnabled: m_Shader.DepthTest,
            comparisonKind: ComparisonKind.LessEqual
        );

        pipelineDescription.RasterizerState = new RasterizerStateDescription(
            cullMode: m_Shader.CullMode,
            fillMode: PolygonFillMode.Solid,
            frontFace: FrontFace.Clockwise,
            depthClipEnabled: true,
            scissorTestEnabled: false
        );

        pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleList;
        pipelineDescription.ResourceLayouts = new ResourceLayout[] {
            factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)
                )
            ),
            factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler, ShaderStages.Fragment)
                )
            )
        };
        
        pipelineDescription.ShaderSet = new ShaderSetDescription(
            vertexLayouts: new VertexLayoutDescription[] { Renderer.vertexLayout },
            shaders: new[] { m_Shader.VertexShader, m_Shader.FragmentShader }
        );

        pipelineDescription.Outputs = Renderer.GraphicsDevice.SwapchainFramebuffer.OutputDescription;
        
        m_Pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
        #endregion
        
        #region Create Resource Sets
        m_ProjViewSet = _graphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                pipelineDescription.ResourceLayouts[0],
                Renderer.ProjectionBuffer,
                Renderer.ViewBuffer,
                m_LocalWorldBuffer
            )
        );
        m_TextureSet = factory.CreateResourceSet(new ResourceSetDescription(
                pipelineDescription.ResourceLayouts[1],
                m_Texture.TextureView,
                Renderer.GraphicsDevice.Aniso4xSampler
            )
        );
        #endregion
        
        m_IsDisposed = false;
    }
    
    // there is definitely a way to improve this setup but it should work for now
    public override void Render(GraphicsDevice gd, CommandList cl)
    {
#if DEBUG
        cl.PushDebugGroup($"TexturedMesh_{ID}_Render");
#endif
        // create and set our world matrix
        //Matrix4x4 origin = Matrix4x4.CreateTranslation(m_LocalOrigin);
        Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(m_Rotation);
        Matrix4x4 scale = Matrix4x4.CreateScale(m_Scale);
        Matrix4x4 worldMatrix = rotation * scale;
        worldMatrix.Translate = m_Position;
        cl.UpdateBuffer(m_LocalWorldBuffer, 0, ref worldMatrix);

        cl.SetPipeline(m_Pipeline);
        cl.SetGraphicsResourceSet(0, m_ProjViewSet);
        cl.SetGraphicsResourceSet(1, m_TextureSet);
        cl.SetVertexBuffer(0, m_Mesh.VertexBuffer);
        cl.SetIndexBuffer(m_Mesh.IndexBuffer, IndexFormat.UInt16);
        cl.DrawIndexed(m_Mesh.IndexCount, 1, 0, 0, 0);
#if DEBUG
        cl.PopDebugGroup();
#endif
    }
}
