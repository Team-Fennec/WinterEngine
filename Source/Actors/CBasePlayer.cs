using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterEngine.Attributes;

namespace WinterEngine.Actors;

[EntityClass("base_player")]
public class CBasePlayer : BaseActor {

    public override void Death() {
        throw new NotImplementedException();
    }

    public override void Render(double deltaTime) {
        throw new NotImplementedException();
    }

    public override void Spawn() {
        throw new NotImplementedException();
    }

    public override void Think(double deltaTime) {
        throw new NotImplementedException();
    }
}
