using Veldrid.Sdl2;
using static WinterEngine.Localization.StringTools;

internal class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        WinterEngine.Core.Engine.PreInit(args);

#if DEBUG
        WinterEngine.Core.Engine.Init();
        WinterEngine.Core.Engine.Run();
#else
        try
        {
            WinterEngine.Core.Engine.Init();
            WinterEngine.Core.Engine.Run();
        }
        catch (Exception e)
        {
            // catch any unhanled exceptions
            unsafe
            {
                Sdl2Native.SDL_ShowSimpleMessageBox(
                    SDL_MessageBoxFlags.Error,
                    "Winter Engine",
                    $"Engine Error:\n{e.ToString()}",
                    null
                );
            }

            if (WinterEngine.Core.Engine.IsRunning)
            {
                // shut down the engine if we're still running
                // WARNING: If we throw on shutdown we won't cleanly exit!
                WinterEngine.Core.Engine.Shutdown();
            }

            return 1;
        }
#endif

        return 0;
    }
}