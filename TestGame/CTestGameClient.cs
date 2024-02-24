using log4net;
using WinterEngine.Core;

namespace TestGame;

public class CTestGameClient : CGameClient {
    public override void ClientStartup() {
        Log.Info("Staring up Client");
    }

    public override void ClientShutdown() {
        Log.Info("Shutting down Client");
    }
}
