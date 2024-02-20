using Foster.Framework;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using log4net;
using log4net.Config;
using System.IO;
using System;
using WinterEngine.Core;

namespace WinterEngineLauncher;

class Launcher
{
	public static void Main(string[] args)
	{
	    // Set up a simple configuration that logs on the console.
        BasicConfigurator.Configure();
		
		Log.Info($"WinterEngine v.{Engine.Version.Major}.{Engine.Version.Minor}.{Engine.Version.Build}");

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
	    

		AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
		{
			HandleError((Exception)e.ExceptionObject);
		};

		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
		CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

		try
		{
			App.Run<Engine>(gameName, 1280, 720);
		}
		catch (Exception e)
		{
			HandleError(e);
		}
	}
	
	private static void HandleError(Exception e)
	{
		// write error to console in case they can see stdout
		Console.WriteLine(e?.ToString() ?? string.Empty);

		// construct a log message
		const string ErrorFileName = "ErrorLog.txt";
		StringBuilder error = new();
		error.AppendLine($"WinterEngine v.{Engine.Version.Major}.{Engine.Version.Minor}.{Engine.Version.Build}");
		error.AppendLine($"Error Log ({DateTime.Now})");
		error.AppendLine($"Call Stack:");
		error.AppendLine(e?.ToString() ?? string.Empty);
		error.AppendLine($"Engine Output:");
		lock (Log.Logs)
			error.AppendLine(Log.Logs.ToString());

		// write to file
		string path = ErrorFileName;
		{
			if (App.Running)
			{
				try
				{
					path = Path.Join(App.UserPath, ErrorFileName);
				}
				catch
				{
					path = ErrorFileName;
				}
			}

			File.WriteAllText(path, error.ToString());
		}

		// open the file
		if (File.Exists(path))
		{
			new Process { StartInfo = new ProcessStartInfo(path) { UseShellExecute = true } }.Start();
		}
	}
}