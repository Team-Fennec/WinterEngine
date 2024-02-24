using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;
using log4net;
using ImGuiNET;
using System.Numerics;
using MathLib;

namespace WinterEngine.Rendering;

public static class Renderer {
    public static GraphicsDevice GraphicsDevice => _graphicsDevice;
    public static ImGuiController ImGuiController => _imguiRend;

    private static readonly ILog log = LogManager.GetLogger(typeof(Renderer));
    
    private static GraphicsDevice _graphicsDevice;
    private static ImGuiController _imguiRend;

    private static DeviceBuffer _projectionBuffer;
    private static DeviceBuffer _viewBuffer;
    private static DeviceBuffer _worldBuffer;

    // you shouldn't need more than 32 shaders, if that, in a frame.
    // if you somehow do, God help you.
    private const int MaxPipelines = 32;
    private static Dictionary<string, Pipeline> _pipelines = new Dictionary<string, Pipeline>();
    private static List<string> _pipelinesInUse = new List<string>();

    // shared between pipelines
    private static GraphicsPipelineDescription pipelineDescription;

    private static ResourceSet _projViewSet;
    private static ResourceSet _worldTextureSet;
    
    private static CommandList _cl;
    private static Vector3 _clearColor = new Vector3(0.45f, 0.55f, 0.6f);

    private static Stack<RenderObject> renderObjects = new Stack<RenderObject>();

    private static readonly VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
        new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
        new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2)
    );

    public static void PushRO(RenderObject ro) {
        renderObjects.Push(ro);
    }

    public static void DrawMesh(MeshHandle mesh) {
        _cl.SetVertexBuffer(0, mesh.VertexBuffer);
        _cl.SetIndexBuffer(mesh.IndexBuffer, IndexFormat.UInt16);
        _cl.SetGraphicsResourceSet(0, _projViewSet);
        _cl.SetGraphicsResourceSet(1, _worldTextureSet);
        _cl.DrawIndexed(mesh.IndexCount, 1, 0, 0, 0);
    }

    public static void UseShader(ShaderHandle shader) {
        // don't recreate the pipeline if it already exists
        if (!_pipelines.ContainsKey(shader.ShaderName)) {
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                shaders: new[] { shader.VertexShader, shader.FragmentShader }
            );

            _pipelines.Add(shader.ShaderName, _graphicsDevice.ResourceFactory.CreateGraphicsPipeline(pipelineDescription));
        }

        // columbo here, uh. Pipebomb?
        _pipelines.TryGetValue(shader.ShaderName, out var pipebomb);
        _cl.SetPipeline(pipebomb);
        // don't allow us to dispose of a pipeline we are in fact using
        _pipelinesInUse.Add(shader.ShaderName);
    }

    public static void UseTexture(TextureHandle texture) {
        _projViewSet = _graphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
            pipelineDescription.ResourceLayouts[0],
            _projectionBuffer,
            _viewBuffer,
            _worldBuffer
            )
        );

        _worldTextureSet = _graphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
            pipelineDescription.ResourceLayouts[1],
            texture.TextureView,
            _graphicsDevice.Aniso4xSampler
            )
        );
    }
    
    public static void Init() {
        log.Info("Initializing GraphicsDevice");
        GraphicsDeviceOptions options = new GraphicsDeviceOptions(
            true,
            PixelFormat.R32_Float,
            false,
            ResourceBindingModel.Improved,
            true,
            true,
            false
        );
        _graphicsDevice = VeldridStartup.CreateGraphicsDevice(Device.Window, options);

        log.Info("Initializing ImGui");

        _imguiRend = new ImGuiController(
            _graphicsDevice,
            _graphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
            Device.Window.Width,
            Device.Window.Height
        );

        log.Info("Creating Veldrid Resources");
        CreateResources();
    }

    static void CreateResources() {
        ResourceFactory factory = _graphicsDevice.ResourceFactory;

        _projectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
        _viewBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
        _worldBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

        pipelineDescription = new GraphicsPipelineDescription();
        pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

        pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
            depthTestEnabled: true,
            depthWriteEnabled: true,
            comparisonKind: ComparisonKind.LessEqual
        );

        pipelineDescription.RasterizerState = new RasterizerStateDescription(
            cullMode: FaceCullMode.Back,
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

        pipelineDescription.Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription;

        _cl = factory.CreateCommandList();
    }

    public static void Shutdown() {
        _graphicsDevice.WaitForIdle();

        log.Info("Disposing Veldrid Resources...");

        foreach (Pipeline pipeline in _pipelines.Values) {
            pipeline.Dispose();
        }
        log.Info("Disposed pipelines");

        _cl.Dispose();
        _graphicsDevice.Dispose();
        _imguiRend.Dispose();
    }

    public static void Render() {
        // check every render object we have into an imgui window for debug purposes
        /*if (ImGui.Begin("Render Object Debug")) {
            foreach (RenderObject ro in renderObjects) {
                ImGui.Text(ro.Name);
            }
            
            ImGui.End();
        }*/
        
        _cl.Begin();

        _cl.UpdateBuffer(_projectionBuffer, 0, Matrix4x4.CreatePerspectiveFieldOfView(
            (float)Angles.Deg2Rad(90),
            (float)960 / 540,
            0.5f,
            9999f));

        _cl.UpdateBuffer(_viewBuffer, 0, Matrix4x4.CreateLookAt(Vector3.UnitZ*4000, Vector3.UnitZ, Vector3.UnitY));

        Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(0, 0, 0);

        _cl.UpdateBuffer(_worldBuffer, 0, ref rotation);

        _cl.SetFramebuffer(_graphicsDevice.MainSwapchain.Framebuffer);

        _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));

        // Send calls to renderer main to render everything
        _cl.ClearDepthStencil(1f);

        // Run through the render objects
        while (renderObjects.Count > 0) {
            renderObjects.First().Render();
            renderObjects.Pop();
        }
        
        _imguiRend.Render(_graphicsDevice, _cl);

        _cl.End();
        _graphicsDevice.SubmitCommands(_cl);
        _graphicsDevice.SwapBuffers(_graphicsDevice.MainSwapchain);
        
        // clear the in use pipelines, we aren't using them anymore
        _pipelinesInUse.Clear();
    }
}
