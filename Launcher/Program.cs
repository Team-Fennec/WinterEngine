using Veldrid.Sdl2;
using log4net;
using log4net.Config;
using System.Reflection;
using WinterEngine;

using static WinterEngine.Translation.StringTools;

internal class Program
{
    private static int Main(string[] args)
    {
        // Set up log4net
        XmlConfigurator.Configure(new FileInfo("logconfig.xml"));
        LogManager.GetLogger("Launcher").Info($"WinterEngine {EngineVersion.Version.Major} patch {EngineVersion.Version.Minor} (build {EngineVersion.Build})");
    
        WinterEngine.Core.Engine.PreInit();
    
        // loop through command line arguments
        string gameName = "";
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-game":
                    if (i == args.Length - 1)
                    {
                        unsafe
                        {
                            Sdl2Native.SDL_ShowSimpleMessageBox(
                                SDL_MessageBoxFlags.Error,
                                "Winter Engine",
                                TRS("engine.error.no_game_provided"),
                                null
                            );
                        }
                        return 1;
                    }
                    gameName = args[i + 1];
                    break;
            }
        }

        if (gameName == "" || !Directory.Exists(gameName))
        {
            unsafe
            {
                Sdl2Native.SDL_ShowSimpleMessageBox(
                    SDL_MessageBoxFlags.Error,
                    "Winter Engine",
                    TRS("engine.error.invalid_game_provided"),
                    null
                );
            }
            return 1;
        }

#if DEBUG
        WinterEngine.Core.Engine.Init(gameName);
        WinterEngine.Core.Engine.Run();
#else
        try {
            WinterEngine.Core.Engine.Init(gameName);
            WinterEngine.Core.Engine.Run();
        } catch(Exception e) {
            // catch any unhanled exceptions
            unsafe {
                Sdl2Native.SDL_ShowSimpleMessageBox(
                    SDL_MessageBoxFlags.Error,
                    "Winter Engine",
                    $"Engine Error:\n{e.ToString()}",
                    null
                );
            }
        
            if (WinterEngine.Core.Engine.IsRunning) {
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