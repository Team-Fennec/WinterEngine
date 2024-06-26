using log4net;
using GortGame.Entities;
using WinterEngine.Core;
using WinterEngine.InputSystem;
using WinterEngine.SceneSystem;

namespace GortGame;

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
        InputAction moveUpVAction = new InputAction("MoveUpV");
        InputAction moveDownVAction = new InputAction("MoveDownV");

        moveUpAction.AddBinding(Veldrid.Key.W);
        moveDownAction.AddBinding(Veldrid.Key.S);
        moveLeftAction.AddBinding(Veldrid.Key.A);
        moveRightAction.AddBinding(Veldrid.Key.D);
        moveUpVAction.AddBinding(Veldrid.Key.Space);
        moveDownVAction.AddBinding(Veldrid.Key.ShiftLeft);
        moveDownVAction.AddBinding(Veldrid.Key.ShiftRight);

        InputManager.RegisterAction(moveUpAction);
        InputManager.RegisterAction(moveDownAction);
        InputManager.RegisterAction(moveLeftAction);
        InputManager.RegisterAction(moveRightAction);
        InputManager.RegisterAction(moveUpVAction);
        InputManager.RegisterAction(moveDownVAction);

        InputManager.SetMouseCapture(true);
        
        // Create test example scene
        m_TestScene = new Scene("Testing Scene");
        m_TestScene.AddEntity(new EntFreeCam() { Name = "Free Cam Player" });
        m_TestScene.AddEntity(new EntStrawberryTest() { Name = "Strawberry" });

        Entity strawberry2 = new EntStrawberryTest("gort.iqm") { Name = "Gort" };
        strawberry2.Transform.LocalPosition = new System.Numerics.Vector3(15, 15, 0);
        m_TestScene.AddEntity(strawberry2);

        SceneManager.LoadScene(m_TestScene);
    }

    public override void Shutdown()
    {
        Log.Info("Shutting down game");
        SceneManager.UnloadScene(m_TestScene);
    }
}
