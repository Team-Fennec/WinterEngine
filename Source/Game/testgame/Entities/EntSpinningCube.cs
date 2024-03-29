using WinterEngine.SceneSystem;
using WinterEngine.RenderSystem;
using Veldrid;
using MathLib;
using WinterEngine.MaterialSystem;
using System.Numerics;
using WinterEngine.Materials;
using WinterEngine.Resource;

namespace TestGame.Entities;

public class EntSpinningCube : Entity, IRenderable
{
    private MeshHandle m_CubeMesh;
    private UnlitVertexColoredMaterial m_Material;

    private Pipeline m_Pipeline;
    private DeviceBuffer m_LocalWorldBuffer;
    private ResourceSet m_ProjViewSet;

    public void CreateDeviceResources(ResourceFactory factory)
    {
        ShaderHandle shader = m_Material.GetHandle();

        m_LocalWorldBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

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

        pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleList;
        pipelineDescription.ResourceLayouts = new ResourceLayout[] {
            factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("Sampler", ResourceKind.Sampler, ShaderStages.Fragment)
                )
            )
        };

        pipelineDescription.ShaderSet = new ShaderSetDescription(
            vertexLayouts: new VertexLayoutDescription[] { Renderer.vertexLayout },
            shaders: new[] { shader.VertexShader, shader.FragmentShader }
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
        cl.SetVertexBuffer(0, m_CubeMesh.VertexBuffer);
        cl.SetIndexBuffer(m_CubeMesh.IndexBuffer, IndexFormat.UInt16);
        cl.DrawIndexed(m_CubeMesh.IndexCount, 1, 0, 0, 0);
#if DEBUG
        cl.PopDebugGroup();
#endif
    }

    public override void Spawn()
    {
        m_Material = ResourceManager.Load<UnlitVertexColoredMaterial>("materials/engine/colored_cube.wmat");

        m_CubeMesh = new MeshHandle(
            new Vertex[]
            {
                // front
                new (new (-2.0f,  2.0f, -2.0f), Vector3.One, RgbaFloat.Blue),
                new (new ( 2.0f,  2.0f, -2.0f), Vector3.One, RgbaFloat.Blue),
                new (new ( 2.0f,  2.0f,  2.0f), Vector3.One, RgbaFloat.Blue),
                new (new (-2.0f,  2.0f,  2.0f), Vector3.One, RgbaFloat.Blue),

                // right
                new (new ( 2.0f,  2.0f, -2.0f), Vector3.One, RgbaFloat.Cyan),
                new (new ( 2.0f, -2.0f, -2.0f), Vector3.One, RgbaFloat.Cyan),
                new (new ( 2.0f, -2.0f,  2.0f), Vector3.One, RgbaFloat.Cyan),
                new (new ( 2.0f,  2.0f,  2.0f), Vector3.One, RgbaFloat.Cyan),

                // back
                new (new ( 2.0f, -2.0f, -2.0f), Vector3.One, RgbaFloat.Green),
                new (new (-2.0f, -2.0f, -2.0f), Vector3.One, RgbaFloat.Green),
                new (new (-2.0f, -2.0f,  2.0f), Vector3.One, RgbaFloat.Green),
                new (new ( 2.0f, -2.0f,  2.0f), Vector3.One, RgbaFloat.Green),

                // left
                new (new (-2.0f, -2.0f, -2.0f), Vector3.One, RgbaFloat.Orange),
                new (new (-2.0f,  2.0f, -2.0f), Vector3.One, RgbaFloat.Orange),
                new (new (-2.0f,  2.0f,  2.0f), Vector3.One, RgbaFloat.Orange),
                new (new (-2.0f, -2.0f,  2.0f), Vector3.One, RgbaFloat.Orange),
                
                // top
                new (new (-2.0f, -2.0f, -2.0f), Vector3.One, RgbaFloat.Pink),
                new (new ( 2.0f, -2.0f, -2.0f), Vector3.One, RgbaFloat.Pink),
                new (new ( 2.0f,  2.0f, -2.0f), Vector3.One, RgbaFloat.Pink),
                new (new (-2.0f,  2.0f, -2.0f), Vector3.One, RgbaFloat.Pink),

                // bottom
                new (new ( 2.0f, -2.0f,  2.0f), Vector3.One, RgbaFloat.Yellow),
                new (new (-2.0f, -2.0f,  2.0f), Vector3.One, RgbaFloat.Yellow),
                new (new (-2.0f,  2.0f,  2.0f), Vector3.One, RgbaFloat.Yellow),
                new (new ( 2.0f,  2.0f,  2.0f), Vector3.One, RgbaFloat.Yellow)
            },
            new ushort[] {
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

        CreateDeviceResources(Renderer.GraphicsDevice.ResourceFactory);

        Transform.LocalScale = new Vector3(2.0f);
        Transform.LocalPosition.X = 20;
    }

    public override void Think(double deltaTime)
    {
        base.Think(deltaTime);

        Transform.LocalEulerRotation.X += (0.2f * (float)deltaTime);
        Transform.LocalEulerRotation.Y += (0.2f * (float)deltaTime);
    }
}
