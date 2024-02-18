using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using log4net;
using log4net.Config;
using System.Text;

internal class Program
{
    private static int Main(string[] args)
    {
        // Set up a simple configuration that logs on the console.
        BasicConfigurator.Configure();

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
                        return 0;
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
            return 0;
        }

        WinterEngine.Core.Engine.Run(gameName);

        return 0;
    }
}