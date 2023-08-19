using DoomGame.Debug;

namespace DoomGame.Main;

class Program
{
	static int Main(string[] args)
	{
		// Initialize logger
		Logger.Init();

		Logger.Log("Main", "Starting game loop...", LogType.Info);
		using (Game game = new Game(800, 600, "DoomGame"))
		{
			game.Run();
		}

		Logger.Log("Main", "Shutting down...", LogType.Info);
		
		Logger.Shutdown();

		return 0;
	}
}