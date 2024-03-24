namespace WinterEngine.SceneSystem.Attributes;

/// <summary>
/// Binds an actor class to an entity class for the map editor & internal entity system.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class EntityClassAttribute : Attribute
{
    public string EntityClass;

    public EntityClassAttribute(string entityClass)
    {
        EntityClass = entityClass;
    }
}
