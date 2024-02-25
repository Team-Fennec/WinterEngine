using log4net;

namespace WinterEngine.Core;
public abstract class CGameServer {
    protected static readonly ILog Log = LogManager.GetLogger("Server");

    public abstract void ServerStartup();
    public abstract void ServerShutdown();
}
