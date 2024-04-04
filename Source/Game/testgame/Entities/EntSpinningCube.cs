using MathLib;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;
using WinterEngine.MaterialSystem;
using WinterEngine.RenderSystem;
using WinterEngine.SceneSystem;

namespace TestGame.Entities;

public class EntSpinningCube : Entity, IRenderable
{
#pragma warning disable CS8618
    private MeshHandle m_CubeMesh;
    private MaterialResource m_Material;

    private Pipeline m_Pipeline;
    private DeviceBuffer m_LocalWorldBuffer;
    private ResourceSet m_ShaderParams;
    private ResourceSet m_ProjViewSet;

    public DisposeCollectorResourceFactory m_Factory { get; set; }
#pragma warning restore

    public void CreateDeviceResources()
    {
        m_Factory = new(Renderer.GraphicsDevice.ResourceFactory);
        ShaderHandle shader = m_Material.GetHandle();

        m_LocalWorldBuffer = m_Factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

        // todo: find a better way to handle this, lmfao
        #region Create Pipeline   
        GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
        pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

        pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
            depthTestEnabled: shader.DepthTest,
            depthWriteEnabled: shader.DepthTest,
            comparisonKind: ComparisonKind.LessEqual
        );

        pipelineDescription.RasterizerState = new RasterizerStateDescription(
            cullMode: shader.CullMode,
            fillMode: PolygonFillMode.Solid,
            frontFace: FrontFace.Clockwise,
            depthClipEnabled: true,
            scissorTestEnabled: false
        );

        List<ResourceLayoutElementDescription> elementDescriptions = new List<ResourceLayoutElementDescription>();
        foreach (ShaderParam param in shader.Params)
        {
            elementDescriptions.Add(new ResourceLayoutElementDescription(param.Name, param.Kind, param.Stage));
        }

        pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleList;
        pipelineDescription.ResourceLayouts = new ResourceLayout[] {
            m_Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("Sampler", ResourceKind.Sampler, ShaderStages.Fragment)
                )
            ),
            m_Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    elementDescriptions.ToArray()
                )
            )
        };

        pipelineDescription.ShaderSet = new ShaderSetDescription(
            vertexLayouts: new VertexLayoutDescription[] { Renderer.vertexLayout },
            shaders: new[] { shader.VertexShader, shader.FragmentShader }
        );

        pipelineDescription.Outputs = Renderer.GraphicsDevice.SwapchainFramebuffer.OutputDescription;

        m_Pipeline = m_Factory.CreateGraphicsPipeline(pipelineDescription);
        #endregion

        #region Create Resource Sets
        m_ProjViewSet = m_Factory.CreateResourceSet(new ResourceSetDescription(
                pipelineDescription.ResourceLayouts[0],
                Renderer.ProjectionBuffer,
                Renderer.ViewBuffer,
                m_LocalWorldBuffer,
                Renderer.GraphicsDevice.Aniso4xSampler
            )
        );

        // go through and add all values
        List<BindableResource> data = new List<BindableResource>();
        foreach (ShaderParam param in shader.Params)
        {
            data.Add(param.Value);
        }

        m_ShaderParams = m_Factory.CreateResourceSet(new ResourceSetDescription(
                pipelineDescription.ResourceLayouts[1],
                data.ToArray()
            )
        );
        #endregion
    }

    public void Render(GraphicsDevice gd, CommandList cl)
    {
#if DEBUG
        cl.PushDebugGroup($"SpinningCube_Render");
#endif
        Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(
            Transform.EulerRotation.X,
            Transform.EulerRotation.Y,
            Transform.EulerRotation.Z
        );
        Matrix4x4 scale = Matrix4x4.CreateScale(Transform.Scale);
        Matrix4x4 worldMatrix = rotation * scale;
        worldMatrix.Translation = Transform.Position;
        cl.UpdateBuffer(m_LocalWorldBuffer, 0, ref worldMatrix);

        cl.SetPipeline(m_Pipeline);
        cl.SetGraphicsResourceSet(0, m_ProjViewSet);
        cl.SetGraphicsResourceSet(1, m_ShaderParams);
        cl.SetVertexBuffer(0, m_CubeMesh.VertexBuffer);
        cl.SetIndexBuffer(m_CubeMesh.IndexBuffer, IndexFormat.UInt16);
        cl.DrawIndexed(m_CubeMesh.IndexCount, 1, 0, 0, 0);
#if DEBUG
        cl.PopDebugGroup();
#endif
    }

    public override void Spawn()
    {
        m_Material = MaterialSystem.Load("engine/colored_cube");

        m_CubeMesh = new MeshHandle(
            new Vertex[]
            {
                // front
                new (new (-2.0f,  2.0f, -2.0f), Vector3.One, RgbaFloat.White, new Vector2(0, 0)),
                new (new ( 2.0f,  2.0f, -2.0f), Vector3.One, RgbaFloat.White, new Vector2(1, 0)),
                new (new ( 2.0f,  2.0f,  2.0f), Vector3.One, RgbaFloat.White, new Vector2(1, 1)),
                new (new (-2.0f,  2.0f,  2.0f), Vector3.One, RgbaFloat.White, new Vector2(0, 1)),

                // right
                new (new ( 2.0f,  2.0f, -2.0f), Vector3.One, RgbaFloat.White, new Vector2(0, 0)),
                new (new ( 2.0f, -2.0f, -2.0f), Vector3.One, RgbaFloat.White, new Vector2(1, 0)),
                new (new ( 2.0f, -2.0f,  2.0f), Vector3.One, RgbaFloat.White, new Vector2(1, 1)),
                new (new ( 2.0f,  2.0f,  2.0f), Vector3.One, RgbaFloat.White, new Vector2(0, 1)),

                // back
                new (new ( 2.0f, -2.0f, -2.0f), Vector3.One, RgbaFloat.White, new Vector2(0, 0)),
                new (new (-2.0f, -2.0f, -2.0f), Vector3.One, RgbaFloat.White, new Vector2(1, 0)),
                new (new (-2.0f, -2.0f,  2.0f), Vector3.One, RgbaFloat.White, new Vector2(1, 1)),
                new (new ( 2.0f, -2.0f,  2.0f), Vector3.One, RgbaFloat.White, new Vector2(0, 1)),

                // left
                new (new (-2.0f, -2.0f, -2.0f), Vector3.One, RgbaFloat.White, new Vector2(0, 0)),
                new (new (-2.0f,  2.0f, -2.0f), Vector3.One, RgbaFloat.White, new Vector2(1, 0)),
                new (new (-2.0f,  2.0f,  2.0f), Vector3.One, RgbaFloat.White, new Vector2(1, 1)),
                new (new (-2.0f, -2.0f,  2.0f), Vector3.One, RgbaFloat.White, new Vector2(0, 1)),
                
                // top
                new (new (-2.0f, -2.0f, -2.0f), Vector3.One, RgbaFloat.White, new Vector2(0, 0)),
                new (new ( 2.0f, -2.0f, -2.0f), Vector3.One, RgbaFloat.White, new Vector2(1, 0)),
                new (new ( 2.0f,  2.0f, -2.0f), Vector3.One, RgbaFloat.White, new Vector2(1, 1)),
                new (new (-2.0f,  2.0f, -2.0f), Vector3.One, RgbaFloat.White, new Vector2(0, 1)),

                // bottom
                new (new ( 2.0f, -2.0f,  2.0f), Vector3.One, RgbaFloat.White, new Vector2(0, 0)),
                new (new (-2.0f, -2.0f,  2.0f), Vector3.One, RgbaFloat.White, new Vector2(1, 0)),
                new (new (-2.0f,  2.0f,  2.0f), Vector3.One, RgbaFloat.White, new Vector2(1, 1)),
                new (new ( 2.0f,  2.0f,  2.0f), Vector3.One, RgbaFloat.White, new Vector2(0, 1))
            },
            new uint[] {
                0, 1, 2,
                2, 3, 0,

                4, 5, 6,
                6, 7, 4,

                8, 9, 10,
                10, 11, 8,

                12, 13, 14,
                14, 15, 12,

                16, 17, 18,
                18, 19, 16,

                20, 21, 22,
                22, 23, 20
            }
        );

        CreateDeviceResources();

        Transform.LocalScale = new Vector3(2.0f);
        Transform.LocalPosition.X = 20;
    }

    public override void Death()
    {
        base.Death();
        m_Factory.DisposeCollector.DisposeAll();
    }

    public override void Think(double deltaTime)
    {
        base.Think(deltaTime);

        Transform.LocalEulerRotation.X += (0.2f * (float)deltaTime);
        Transform.LocalEulerRotation.Y += (0.2f * (float)deltaTime);
    }
}
