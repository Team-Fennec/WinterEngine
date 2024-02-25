using log4net;
using System.Timers;
using System.Diagnostics;
using WinterEngine.Actors;
using Timer = System.Timers.Timer;

namespace WinterEngine.Core;
public abstract class CGameServer {
    public const int FIXED_SERVER_TIC = 60; // server ticrate fixed to 60

    protected static readonly ILog Log = LogManager.GetLogger("Server");
    public List<BaseServerActor> Actors = new List<BaseServerActor>();

    private Timer serverTicTimer;

    public abstract void ServerStartup();
    public abstract void ServerShutdown();

    public void Startup() {
        Log.Info("Starting up server");

        ServerStartup();

        serverTicTimer = new Timer(1000 / FIXED_SERVER_TIC);
        // Hook up the Elapsed event for the timer. 
        serverTicTimer.Elapsed += ServerUpdate;
        serverTicTimer.AutoReset = true;
        serverTicTimer.Enabled = true;
        deltaTimer = Stopwatch.StartNew();
    }

    public void Shutdown() {
        Log.Info("Shutting down server");
        serverTicTimer.Enabled = false;

        // destroy all actors
        foreach (BaseServerActor serverActor in Actors) {
            serverActor.Death();
            Actors.Remove(serverActor);
        }

        // shutdown the rest of the server
        ServerShutdown();
    }

    Stopwatch deltaTimer;
    public void ServerUpdate(Object source, ElapsedEventArgs e) {
        float deltaTime = deltaTimer.ElapsedTicks / (float)Stopwatch.Frequency;
        deltaTimer.Restart();
        
        foreach (BaseServerActor serverActor in Actors) {
            serverActor.Think(deltaTime);
        }
    }

    public void ServerReceive() {

    }

    public void ServerSend() {

    }
}
