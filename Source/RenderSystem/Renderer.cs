//#define THIRDPERSON_CAMERA
using ImGuiNET;
using log4net;
using MathLib;
using System.Numerics;
using Veldrid;
using Veldrid.StartupUtilities;
using WinterEngine.SceneSystem;
#if HAS_PROFILING
using WinterEngine.Diagnostics;
#endif

namespace WinterEngine.RenderSystem;

public static class Renderer
{
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
    private static Vector3 _clearColor = new Vector3(0.25f, 0.25f, 0.25f);

    public static readonly VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
        new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
        new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
        new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
        new VertexElementDescription("Joint", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
        new VertexElementDescription("Weight", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4)
    );

    public static void Init()
    {
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
#if LINUX
        _graphicsDevice = VeldridStartup.CreateGraphicsDevice(Device.Window, options, GraphicsBackend.Vulkan);
#else
        _graphicsDevice = VeldridStartup.CreateGraphicsDevice(Device.Window, options);
#endif

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

    static void CreateResources()
    {
        ResourceFactory factory = _graphicsDevice.ResourceFactory;

        _projectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
        _viewBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
        _worldBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

        _cl = factory.CreateCommandList();
    }

    public static void Shutdown()
    {
        _graphicsDevice.WaitForIdle();

        log.Info("Disposing Resources...");

        _cl.Dispose();
        _graphicsDevice.Dispose();
        _imguiRend.Dispose();
    }

    public static void Render(float deltaTime)
    {
#if HAS_PROFILING
        Profiler.PushProfile("RenderWorld");
#endif
        _cl.Begin();

#if DEBUG
        _cl.PushDebugGroup("GlobalWorldMatrix");
#endif
        _cl.UpdateBuffer(_projectionBuffer, 0, Matrix4x4.CreatePerspectiveFieldOfView(
            (float)Angles.Deg2Rad(90),
            (float)Device.Window.Width / Device.Window.Height,
            0.5f,
            9999f));

        Vector3 m_CamPos = SceneManager.ActiveCamera.Position;
        Vector3 m_CamAng = SceneManager.ActiveCamera.Rotation;

#if THIRDPERSON_CAMERA
        Vector3 lookFrom = new Vector3(
            m_CamPos.X + 30.0f * (float)Math.Cos(Angles.Deg2Rad(m_CamAng.Z)),
            m_CamPos.Y - 30.0f * (float)Math.Sin(Angles.Deg2Rad(m_CamAng.Z)),
            m_CamPos.Z - 30.0f * (float)Math.Sin(Angles.Deg2Rad(m_CamAng.X))
        );
        _cl.UpdateBuffer(_viewBuffer, 0, Matrix4x4.CreateLookAt(lookFrom, m_CamPos, Vector3.UnitZ));
#else
        Vector3 lookAt = new Vector3(
            m_CamPos.X + (float)Math.Cos(Angles.Deg2Rad(m_CamAng.Z)),
            m_CamPos.Y - (float)Math.Sin(Angles.Deg2Rad(m_CamAng.Z)),
            m_CamPos.Z - (float)Math.Sin(Angles.Deg2Rad(m_CamAng.X))
        );
        _cl.UpdateBuffer(_viewBuffer, 0, Matrix4x4.CreateLookAt(m_CamPos, lookAt, Vector3.UnitZ));
#endif

        Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(m_CamAng.Z, m_CamAng.X, m_CamAng.Y);
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
        if (SceneManager.CurrentScene != null)
        {
            foreach (var entity in SceneManager.CurrentScene.Entities)
            {
                IRenderable rendEnt = entity as IRenderable;
                if (rendEnt != null)
                {
                    rendEnt.Render(_graphicsDevice, _cl);
                }
            }
        }

        _imguiRend.Render(_graphicsDevice, _cl);
#if DEBUG
        _cl.PopDebugGroup();
#endif

        _cl.End();
        _graphicsDevice.SubmitCommands(_cl);
        _graphicsDevice.SwapBuffers(_graphicsDevice.MainSwapchain);
#if HAS_PROFILING
        Profiler.PopProfile();
#endif
    }
}
