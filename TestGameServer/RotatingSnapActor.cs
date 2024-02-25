using WinterEngine;
using WinterEngine.Actors;
using WinterEngine.Attributes;

namespace TestGameServer;

[EntityClass("test_snaprotate")]
public class RotatingSnapActor : BaseServerActor {
    public override void Death() {
        throw new NotImplementedException();
    }

    public override void Spawn() {
        throw new NotImplementedException();
    }

    public override void Think(double deltaTime) {
        throw new NotImplementedException();
    }
}
