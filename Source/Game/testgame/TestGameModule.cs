using log4net;
using TestGame.Entities;
using WinterEngine.Core;
using WinterEngine.SceneSystem;

namespace TestGame;

public class TestGameModule : GameModule
{
    Scene m_TestScene;

    public override void Startup()
    {
        Log.Info("Staring up game");

        // Create test example scene
        m_TestScene = new Scene("Testing Scene");
        m_TestScene.AddEntity(new EntFreeCam() { Name = "Free Cam Player" });
        SceneManager.LoadScene(m_TestScene);
    }

    public override void Shutdown()
    {
        Log.Info("Shutting down game");
        SceneManager.UnloadScene(m_TestScene);
    }
}
