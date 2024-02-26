using System.Numerics;
using WinterEngine.Attributes;

namespace WinterEngine.Actors;

[EntityClass("actor")]
public abstract class BaseActor {
    [EntityKV("globalname")]
    public string globalName;
    [EntityKV("position")]
    public Vector3 position;
    [EntityKV("rotation")]
    public Vector3 rotation; // I fucking hate quaternions

    public abstract void Spawn();
    public abstract void Death();
    public abstract void Think(double deltaTime);
    public abstract void Render(double deltaTime);
}