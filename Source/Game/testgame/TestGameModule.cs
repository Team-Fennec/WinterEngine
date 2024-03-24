using log4net;
using TestGame.Entities;
using WinterEngine.Core;
using WinterEngine.InputSystem;
using WinterEngine.SceneSystem;

namespace TestGame;

public class TestGameModule : GameModule
{
    Scene m_TestScene;

    public override void Startup()
    {
        Log.Info("Staring up game");

        // register freecam actions
        InputAction moveUpAction = new InputAction("MoveUp");
        InputAction moveDownAction = new InputAction("MoveDown");
        InputAction moveLeftAction = new InputAction("MoveLeft");
        InputAction moveRightAction = new InputAction("MoveRight");
        InputAction lookLeftAction = new InputAction("LookLeft");
        InputAction lookRightAction = new InputAction("LookRight");

        moveUpAction.AddBinding(Veldrid.Key.W);
        moveDownAction.AddBinding(Veldrid.Key.S);
        moveLeftAction.AddBinding(Veldrid.Key.A);
        moveRightAction.AddBinding(Veldrid.Key.D);
        lookLeftAction.AddBinding(Veldrid.Key.Left);
        lookRightAction.AddBinding(Veldrid.Key.Right);

        InputManager.RegisterAction(moveUpAction);
        InputManager.RegisterAction(moveDownAction);
        InputManager.RegisterAction(moveLeftAction);
        InputManager.RegisterAction(moveRightAction);
        InputManager.RegisterAction(lookLeftAction);
        InputManager.RegisterAction(lookRightAction);

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
