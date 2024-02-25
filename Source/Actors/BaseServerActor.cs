using System.Numerics;
using WinterEngine.Attributes;

namespace WinterEngine.Actors;

[EntityClass("actor")]
public abstract class BaseServerActor {
    [EntityKV("globalname")]
    public string globalName;
    [EntityKV("position")]
    public Vector3 position;
    [EntityKV("rotation")]
    public Vector3 rotation; // I fucking hate quaternions
}
