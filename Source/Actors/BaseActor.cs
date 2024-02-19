using Microsoft.Xna.Framework;

namespace WinterEngine.Actors;

public abstract class BaseActor
{
    public Vector3 position;
    public Quaternion rotation;

    public abstract void Spawn();
    public abstract void Death();
	public abstract void Think(GameTime gameTime);
    public abstract void Render(GameTime gameTime);
}