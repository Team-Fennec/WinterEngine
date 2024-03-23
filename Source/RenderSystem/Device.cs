using System;
using Veldrid.StartupUtilities;
using Veldrid.Sdl2;

namespace WinterEngine.RenderSystem;

// does this really count as rendering related?
public static class Device {
    public static Sdl2Window Window => _window;

    private static readonly ILog log = LogManager.GetLogger("Device");
    private static Sdl2Window _window;

    public static void Init(string windowName) {
        log.Info("Initializing Veldrid SDL2 Window...");
        WindowCreateInfo windowCI = new WindowCreateInfo() {
            X = 100,
            Y = 100,
            WindowWidth = 960,
            WindowHeight = 540,
            WindowTitle = windowName
        };
        _window = VeldridStartup.CreateWindow(ref windowCI);
    }
}
