using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;
using ImGuiNET;
using System.Numerics;
using MathLib;

namespace WinterEngine.RenderSystem;

public static class Renderer {
    public static GraphicsDevice GraphicsDevice => _graphicsDevice;
    public static ImGuiController ImGuiController => _imguiRend;

    private static readonly ILog log = LogManager.GetLogger("Renderer");
    
    private static GraphicsDevice _graphicsDevice;
    private static ImGuiController _imguiRend;

    // HACK: this really shouldn't be public
    public static DeviceBuffer ProjectionBuffer => _projectionBuffer;
    public static DeviceBuffer ViewBuffer => _viewBuffer;
    private static DeviceBuffer _projectionBuffer;
    private static DeviceBuffer _viewBuffer;
    private static DeviceBuffer _worldBuffer;
    
    private static CommandList _cl;
    private static Vector3 _clearColor = new Vector3(0.45f, 0.55f, 0.6f);
    
    private static List<RenderObject> m_Renderables = new List<RenderObject>();

    public static readonly VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
        new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
        new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
        new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2)
    );

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
        
        _cl = factory.CreateCommandList();
    }
    
    public static void AddRenderObject()

    public static void Shutdown() {
        _graphicsDevice.WaitForIdle();

        log.Info("Disposing Resources...");

        foreach (RenderObject ro in m_Renderables)
        {
            // todo: should we clear resources whenever an RO is removed from the list?
            ro.DisposeResources();
        }

        _cl.Dispose();
        _graphicsDevice.Dispose();
        _imguiRend.Dispose();
    }

    public static void Render() {
        _cl.Begin();

#if DEBUG
        _cl.PushDebugGroup("GlobalWorldMatrix");
#endif
        _cl.UpdateBuffer(_projectionBuffer, 0, Matrix4x4.CreatePerspectiveFieldOfView(
            (float)Angles.Deg2Rad(90),
            (float)Device.Window.Width / Device.Window.Height,
            0.5f,
            9999f));
        _cl.UpdateBuffer(_viewBuffer, 0, Matrix4x4.CreateLookAt(Vector3.UnitZ*4000, Vector3.UnitZ, Vector3.UnitY));
        Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(0, 0, 0);
        _cl.UpdateBuffer(_worldBuffer, 0, ref rotation);
#if DEBUG
        _cl.PopDebugGroup();
#endif
        
        _cl.SetFramebuffer(_graphicsDevice.MainSwapchain.Framebuffer);
        _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));

        // Send calls to renderer main to render everything
        _cl.ClearDepthStencil(1f);
        
#if DEBUG
        _cl.PushDebugGroup("RenderScene");
#endif
        foreach (RenderObject ro in m_Renderables)
        {
            ro.Render(_graphicsDevice, _cl);
        }
        
        _imguiRend.Render(_graphicsDevice, _cl);
#if DEBUG
        _cl.PopDebugGroup();
#endif

        _cl.End();
        _graphicsDevice.SubmitCommands(_cl);
        _graphicsDevice.SwapBuffers(_graphicsDevice.MainSwapchain);
    }
}
