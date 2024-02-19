using log4net;
using log4net.Config;
using System.Text;
using System.IO;
using System;

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
                        Console.WriteLine("No game name provided! Pass -game <gamename> on start to load a game.");
                        return 0;
                    }
                    gameName = args[i + 1];
                    break;
            }
        }

        if (gameName == "" || !Directory.Exists(gameName))
        {
            Console.WriteLine("No valid game name provided! Pass -game <gamename> on start to load a game.");
            return 0;
        }

        using var engine = new WinterEngine.Core.Engine(gameName);
        engine.Run();
        
        return 0;
    }
}