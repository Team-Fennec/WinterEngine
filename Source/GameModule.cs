using log4net;
using WinterEngine.Actors;

namespace WinterEngine.Core;

public abstract class GameModule {
    protected static readonly ILog Log = LogManager.GetLogger("Game");
    
    public List<BaseActor> Actors = new List<BaseActor>();

    public abstract void Startup();
    public abstract void Shutdown();
}
