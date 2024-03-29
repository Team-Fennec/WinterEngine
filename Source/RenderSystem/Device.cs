using System;
using Veldrid.StartupUtilities;
using Veldrid.Sdl2;
using log4net;

namespace WinterEngine.RenderSystem;

public static class Device
{
    private static readonly ILog log = LogManager.GetLogger("Device");
    
    public static Sdl2Window Window => m_Window;
    private static Sdl2Window m_Window;

    public static void Init(string windowName)
    {
        log.Info("Initializing Veldrid SDL2 Window...");
        WindowCreateInfo windowCI = new WindowCreateInfo()
        {
            X = 100,
            Y = 100,
            WindowWidth = 960,
            WindowHeight = 540,
            WindowTitle = windowName
        };
        m_Window = VeldridStartup.CreateWindow(ref windowCI);
    }
}
