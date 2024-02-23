using System.Numerics;

namespace WinterEngine.Actors;

public abstract class BaseActor
{
    public Vector3 position;
    public Quaternion rotation;

    public abstract void Spawn();
    public abstract void Death();
	public abstract void Think(double deltaTime);
    public abstract void Render(double deltaTime);
}