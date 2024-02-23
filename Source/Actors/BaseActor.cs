using System.Numerics;

namespace WinterEngine.Actors;

public abstract class BaseActor
{
    public Vector3 position;
    public Vector3 rotation; // I fucking hate quaternions

    public abstract void Spawn();
    public abstract void Death();
	public abstract void Think(double deltaTime);
    public abstract void Render(double deltaTime);
}