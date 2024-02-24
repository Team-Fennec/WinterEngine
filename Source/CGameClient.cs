using log4net;

namespace WinterEngine.Core; 
public abstract class CGameClient {
    protected static readonly ILog Log = LogManager.GetLogger("Client");

    public abstract void ClientStartup();
    public abstract void ClientShutdown();
}
