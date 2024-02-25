using WinterEngine;
using WinterEngine.Actors;
using WinterEngine.Rendering;

namespace TestGame;
public class RotatingSnap : BaseActor, IHasModel {


    public override void Death() {
        throw new NotImplementedException();
    }

    public override void Render(double deltaTime) {
        Renderer.PushRO();
    }

    public override void Spawn() {
        
    }

    public override void Think(double deltaTime) {
        throw new NotImplementedException();
    }
}
