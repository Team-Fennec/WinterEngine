using Veldrid.Sdl2;
using log4net;
using log4net.Config;
using System.Reflection;
using WinterEngine;

internal class Program
{
    private static int Main(string[] args)
    {
        // Set up log4net
        XmlConfigurator.Configure(new FileInfo("logconfig.xml"));

        LogManager.GetLogger("Launcher").Info($"WinterEngine {EngineVersion.Version.Major} patch {EngineVersion.Version.Minor} (build {EngineVersion.Build})");


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
                                "WinterEngine",
                                "No game name provided! Pass -game <gamename> on start to load a game.",
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
                    "WinterEngine",
                    "No valid game provided! Pass -game <gamename> on start to load a game.",
                    null
                );
            }
            return 1;
        }

        WinterEngine.Core.Engine.Init(gameName);
        return WinterEngine.Core.Engine.Run();
    }
}