
namespace TestGame;

[EntityClass("test_snaprotate")]
public class RotatingSnapActor : BaseActor, IHasModel {
    public override void Death() {
        throw new NotImplementedException();
    }

    public override void Spawn() {
        // load in model 
        throw new NotImplementedException();
    }

    public override void Think(double deltaTime) {
        throw new NotImplementedException();
    }
    
    public override void Render(double deltaTime) {
        //Renderer.PushRO();
    }
}
