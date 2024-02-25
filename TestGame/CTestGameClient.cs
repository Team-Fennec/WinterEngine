using log4net;
using WinterEngine.Core;

namespace TestGameClient;

public class CTestGameClient : CGameClient {
    public override void ClientStartup() {
        Log.Info("Staring up Client");
    }

    public override void ClientShutdown() {
        Log.Info("Shutting down Client");
    }
}
