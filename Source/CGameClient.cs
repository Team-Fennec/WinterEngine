using log4net;
using WinterEngine.Actors;

namespace WinterEngine.Core; 
public abstract class CGameClient {
    protected static readonly ILog Log = LogManager.GetLogger("Client");
    public List<BaseActor> Actors = new List<BaseActor>();
    public abstract void ClientStartup();
    public abstract void ClientShutdown();

    public void ClientReceive() {

    }

    public void ClientSend() {

    }
}
