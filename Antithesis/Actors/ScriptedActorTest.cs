using Microsoft.Xna.Framework;
using WinterEngine.Actors;

public class ScriptedActorTest : BaseActor {
    public override void Spawn() {
        Console.WriteLine("Spawned in actor!");
    }

    public override void Death() {
        Console.WriteLine("Killed actor!");
    }

    public override void Think(GameTime gameTime) {

    }

    public override void Render(GameTime gameTime) {
        
    }
}