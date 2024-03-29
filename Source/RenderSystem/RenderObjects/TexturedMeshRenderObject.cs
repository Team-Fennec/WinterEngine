using Veldrid;
using System.Numerics;
using MathLib;

namespace WinterEngine.RenderSystem;

public class TexturedMeshRO : RenderObject
{
    // we hold onto our handle objects since some can be updated
    public ShaderHandle Shader;
    public MeshHandle Mesh;

    private Vector3 m_LocalOrigin;
    private Vector3 m_Position;
    private Vector3 m_Rotation;
    private Vector3 m_Scale;

    private Pipeline m_Pipeline;
    private DeviceBuffer m_LocalWorldBuffer;
    private ResourceSet m_ShaderParams;
    private ResourceSet m_ProjViewSet;

    // CreateDeviceResources assigns a value to these.
#pragma warning disable CS8618
    public TexturedMeshRO(string name, ShaderHandle Shader, MeshHandle Mesh)
    {
        Name = name;
        // Assign our data
        this.Shader = Shader;
        this.Mesh = Mesh;

        m_LocalOrigin = Vector3.Zero;
        m_Position = Vector3.Zero;
        m_Rotation = Vector3.Zero;
        m_Scale = Vector3.One;

        // call to create resources
        CreateDeviceResources();
    }
#pragma warning restore

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
        m_Pipeline.Dispose();
        m_ShaderParams.Dispose();
        m_ProjViewSet.Dispose();
        m_LocalWorldBuffer.Dispose();
    }

    public override void CreateDeviceResources()
    {
        ResourceFactory factory = Renderer.GraphicsDevice.ResourceFactory;

        #region Create Pipeline   
        GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
        pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

        pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
            depthTestEnabled: Shader.DepthTest,
            depthWriteEnabled: Shader.DepthTest,
            comparisonKind: ComparisonKind.LessEqual
        );

        pipelineDescription.RasterizerState = new RasterizerStateDescription(
            cullMode: Shader.CullMode,
            fillMode: PolygonFillMode.Solid,
            frontFace: FrontFace.Clockwise,
            depthClipEnabled: true,
            scissorTestEnabled: false
        );

        List<ResourceLayoutElementDescription> elementDescriptions = new List<ResourceLayoutElementDescription>();
        foreach (ShaderParam param in Shader.Params)
        {
            elementDescriptions.Add(new ResourceLayoutElementDescription(param.Name, param.Kind, param.Stage));
        }

        pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleList;
        pipelineDescription.ResourceLayouts = new ResourceLayout[] {
            factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("Sampler", ResourceKind.Sampler, ShaderStages.Fragment)
                )
            ),
            factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    elementDescriptions.ToArray()
                )
            )
        };

        pipelineDescription.ShaderSet = new ShaderSetDescription(
            vertexLayouts: new VertexLayoutDescription[] { Renderer.vertexLayout },
            shaders: new[] { Shader.VertexShader, Shader.FragmentShader }
        );

        pipelineDescription.Outputs = Renderer.GraphicsDevice.SwapchainFramebuffer.OutputDescription;

        m_Pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
        #endregion

        #region Create Resource Sets
        m_ProjViewSet = Renderer.GraphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                pipelineDescription.ResourceLayouts[0],
                Renderer.ProjectionBuffer,
                Renderer.ViewBuffer,
                m_LocalWorldBuffer,
                Renderer.GraphicsDevice.Aniso4xSampler
            )
        );

        // go through and add all values
        List<BindableResource> data = new List<BindableResource>();
        foreach (ShaderParam param in Shader.Params)
        {
            data.Add(param.Value);
        }

        m_ShaderParams = factory.CreateResourceSet(new ResourceSetDescription(
                pipelineDescription.ResourceLayouts[1],
                data.ToArray()
            )
        );
        #endregion
    }

    // there is definitely a way to improve this setup but it should work for now
    public override void Render(GraphicsDevice gd, CommandList cl)
    {
#if DEBUG
        cl.PushDebugGroup($"TexturedMesh_{ID}_Render");
#endif
        // create and set our world matrix
        //Matrix4x4 origin = Matrix4x4.CreateTranslation(m_LocalOrigin);
        Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(m_Rotation.X, m_Rotation.Y, m_Rotation.Z);
        Matrix4x4 scale = Matrix4x4.CreateScale(m_Scale);
        Matrix4x4 worldMatrix = rotation * scale;
        worldMatrix.Translation = m_Position;
        cl.UpdateBuffer(m_LocalWorldBuffer, 0, ref worldMatrix);

        cl.SetPipeline(m_Pipeline);
        cl.SetGraphicsResourceSet(0, m_ProjViewSet);
        cl.SetGraphicsResourceSet(1, m_ShaderParams);
        cl.SetVertexBuffer(0, Mesh.VertexBuffer);
        cl.SetIndexBuffer(Mesh.IndexBuffer, IndexFormat.UInt16);
        cl.DrawIndexed(Mesh.IndexCount, 1, 0, 0, 0);
#if DEBUG
        cl.PopDebugGroup();
#endif
    }
}
