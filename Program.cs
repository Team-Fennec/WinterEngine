using OpenTK.Windowing.Desktop;

using DoomGame.Debug;

namespace DoomGame.Main;

class Program
{
	static int Main(string[] args)
	{
		// Initialize logger
		Logger.Init();

		NativeWindowSettings windowSettings = new() {
			Title = "DoomGame",
			Size = (800, 600),
			MaximumSize = (800, 600),
			MinimumSize = (800, 600)
		};

		Logger.Log("Main", "Starting game loop...", LogType.Info);

		using (Game game = new Game(windowSettings))
		{
			game.Run();
		}

		Logger.Log("Main", "Shutting down...", LogType.Info);
		
		Logger.Shutdown();

		return 0;
	}
}