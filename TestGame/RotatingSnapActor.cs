﻿using WinterEngine;
using WinterEngine.Actors;
using WinterEngine.Attributes;
using WinterEngine.Rendering;

namespace TestGameClient;

[EntityClass("test_snaprotate")]
public class RotatingSnapActor : BaseActor {


    public override void Death() {
        throw new NotImplementedException();
    }

    public override void Render(double deltaTime) {
        //Renderer.PushRO();
    }

    public override void Spawn() {
        
    }

    public override void Think(double deltaTime) {
        throw new NotImplementedException();
    }
}