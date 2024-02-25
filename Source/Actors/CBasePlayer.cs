using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterEngine.Attributes;

namespace WinterEngine.Actors;

[EntityClass("base_player")]
public class CBasePlayer : BaseActor {
    public override void Render(double deltaTime) {
        throw new NotImplementedException();
    }
}
