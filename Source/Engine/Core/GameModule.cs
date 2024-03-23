namespace WinterEngine.Core;

public abstract class GameModule
{
    protected static readonly ILog Log = LogManager.GetLogger("Game");

    public abstract void Startup();
    public abstract void Shutdown();
}
