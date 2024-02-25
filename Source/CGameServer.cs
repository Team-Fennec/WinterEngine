using log4net;
using WinterEngine.Actors;

namespace WinterEngine.Core;
public abstract class CGameServer {
    protected static readonly ILog Log = LogManager.GetLogger("Server");
    public List<BaseServerActor> Actors = new List<BaseServerActor>();
    public abstract void ServerStartup();
    public abstract void ServerShutdown();

    public void ServerReceive() {

    }

    public void ServerSend() {

    }
}
