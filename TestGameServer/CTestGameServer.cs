using log4net;
using WinterEngine.Core;

namespace TestGameClient;

public class CTestGameServer : CGameServer {
    public override void ServerStartup() {
        Log.Info("Staring up Server");
    }

    public override void ServerShutdown() {
        Log.Info("Shutting down Server");
    }
}
