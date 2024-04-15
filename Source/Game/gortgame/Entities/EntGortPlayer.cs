using WinterEngine.Data;
using WinterEngine.RenderSystem;
using WinterEngine.Resource;
using WinterEngine.SceneSystem;
using WinterEngine.SceneSystem.Attributes;
using WinterEngine.InputSystem;
using System.Numerics;
using MathLib;
using Veldrid;
using Veldrid.Utilities;

namespace GortGame.Entities;

internal class EntGortPlayer : Entity
{
	struct MeshRenderData
    {
        public ModelResource.MeshPrimitive Mesh;
        public Pipeline Pipeline;
        public ResourceSet ShaderResources;
        public ResourceSet ProjViewSet;
    }

    public string CurrentAnimation = "idle";
    public float FacingAngle = 0.0f;

#pragma warning disable CS8618
    private ModelResource m_Model;

    private List<MeshRenderData> m_RenderData = new List<MeshRenderData>();
    private DeviceBuffer m_LocalWorldBuffer;
    private ResourceSet m_ProjViewSet;
    private DeviceBuffer m_blankMatrix;

    public DisposeCollectorResourceFactory m_Factory { get; set; }
#pragma warning restore

    public override void Spawn()
    {
        try
        {
            m_Model = ResourceManager.Load<IQMModelResource>($"models/gort.iqm");
        }
        catch
        {
            m_Model = ResourceManager.Load<GLBModelResource>("models/engine/error.glb");
        }
        CreateDeviceResources();
    }

    public void CreateDeviceResources()
    {
        m_Factory = new(Renderer.GraphicsDevice.ResourceFactory);

        #region Create Shared Resources
        m_LocalWorldBuffer = m_Factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
        m_blankMatrix = m_Factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

        ResourceLayout projViewLayout = m_Factory.CreateResourceLayout(
            new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("JointBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("Sampler", ResourceKind.Sampler, ShaderStages.Fragment)
            )
        );
        #endregion

        #region Create Render Data
        foreach (ModelResource.MeshPrimitive primitive in m_Model.Primitives)
        {
            MeshRenderData renderData = new MeshRenderData();
            renderData.Mesh = primitive;
            ShaderHandle shader = primitive.Material.GetHandle();

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
                projViewLayout,
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

            renderData.Pipeline = m_Factory.CreateGraphicsPipeline(pipelineDescription);

            renderData.ProjViewSet = m_Factory.CreateResourceSet(new ResourceSetDescription(
                    projViewLayout,
                    Renderer.ProjectionBuffer,
                    Renderer.ViewBuffer,
                    m_LocalWorldBuffer,
                    m_blankMatrix,
                    Renderer.GraphicsDevice.Aniso4xSampler
                )
            );

            // go through and add all values
            List<BindableResource> data = new List<BindableResource>();
            foreach (ShaderParam param in shader.Params)
            {
                data.Add(param.Value);
            }

            renderData.ShaderResources = m_Factory.CreateResourceSet(new ResourceSetDescription(
                    pipelineDescription.ResourceLayouts[1],
                    data.ToArray()
                )
            );

            m_RenderData.Add(renderData);
        }
        #endregion
    }

    public void Render(GraphicsDevice gd, CommandList cl)
    {
#if DEBUG
        cl.PushDebugGroup($"Entity_{Name}_Render");
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

       	cl.SetPipeline(renderData[0].Pipeline);
        cl.SetGraphicsResourceSet(0, renderData[0].ProjViewSet);
        cl.SetGraphicsResourceSet(1, renderData[0].ShaderResources);

#if DEBUG
        int rdIndex = 0;
        foreach (MeshRenderData renderData in m_RenderData)
        {

            cl.PushDebugGroup($"{Name}_RenderData_{rdIndex}");
#else
    	foreach (MeshRenderData renderData in m_RenderData)
    	{
#endif
	            cl.SetVertexBuffer(0, renderData.Mesh.Handle.VertexBuffer);
	            cl.SetIndexBuffer(renderData.Mesh.Handle.IndexBuffer, IndexFormat.UInt32);
	            cl.DrawIndexed(renderData.Mesh.Handle.IndexCount, 1, 0, 0, 0);
#if DEBUG
            	cl.PopDebugGroup();
            	rdIndex++;
#endif
        	}
#if DEBUG
        	cl.PopDebugGroup();
#endif
        }
    }

	public override void Think(double deltaTime)
    {
        // update components
        base.Think(deltaTime);

        Vector3 moveDelta = new Vector3(
            (float)Math.Cos(Angles.Deg2Rad(Transform.LocalEulerRotation.Z)) * 12,
            (float)Math.Sin(Angles.Deg2Rad(Transform.LocalEulerRotation.Z)) * 12,
			0.0f
        );

        // input
        if (InputManager.ActionCheck("MoveUp"))
        {
            Transform.LocalPosition.X += (float)(moveDelta.X * deltaTime);
            Transform.LocalPosition.Y -= (float)(moveDelta.Y * deltaTime);
        }
        if (InputManager.ActionCheck("MoveDown"))
        {
            Transform.LocalPosition.X -= (float)(moveDelta.X * deltaTime);
            Transform.LocalPosition.Y += (float)(moveDelta.Y * deltaTime);
        }
        if (InputManager.ActionCheck("MoveLeft"))
        {
            Transform.LocalPosition.X += (float)(moveDelta.Y * deltaTime);
            Transform.LocalPosition.Y += (float)(moveDelta.X * deltaTime);
        }
        if (InputManager.ActionCheck("MoveRight"))
        {
            Transform.LocalPosition.X -= (float)(moveDelta.Y * deltaTime);
            Transform.LocalPosition.Y -= (float)(moveDelta.X * deltaTime);
        }

        Transform.LocalEulerRotation.X -= (float)((InputManager.GetMouseDelta().Y * 30.0) * deltaTime);
        Transform.LocalEulerRotation.Z -= (float)((InputManager.GetMouseDelta().X * 30.0) * deltaTime);
        Transform.LocalEulerRotation.X = Math.Clamp(Transform.LocalEulerRotation.X, -90.0f, 90.0f);

        // set the camera to our position
        SceneManager.ActiveCamera.Position = Transform.Position;
        SceneManager.ActiveCamera.Rotation = Transform.EulerRotation;
    }
}
