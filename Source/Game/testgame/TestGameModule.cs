using log4net;
using WinterEngine.Core;

namespace TestGame;

public class TestGameModule : GameModule
{
    public override void Startup()
    {
        Log.Info("Staring up game");
    }

    public override void Shutdown()
    {
        Log.Info("Shutting down game");
    }
}
