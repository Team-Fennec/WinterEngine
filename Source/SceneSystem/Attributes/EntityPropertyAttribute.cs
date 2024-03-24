namespace WinterEngine.SceneSystem.Attributes;

/// <summary>
/// Binds an actor class to an entity class for the map editor & internal entity system.
/// </summary>
[AttributeUsage(AttributeTargets.Field|AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class EntityPropertyAttribute : Attribute
{
    public string PropName;

    public EntityPropertyAttribute(string propName)
    {
        PropName = propName;
    }
}
