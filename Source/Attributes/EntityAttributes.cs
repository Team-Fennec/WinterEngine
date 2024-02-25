using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinterEngine.Attributes;

/// <summary>
/// Binds an actor class to an entity class in the map editor and internal entity system.
/// 
/// Binding both a server and client entity to the same name is extremely important.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class EntityClassAttribute : Attribute {
    public string EntityClass;

    public EntityClassAttribute(string entityClass) {
        EntityClass = entityClass;
    }
}


/// <summary>
/// Binds a property to a KV value for the internal entity system & map editor.
/// </summary>
[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class EntityKVAttribute : Attribute {
    public string Key;
    
    public EntityKVAttribute(string key) {
        Key = key;
    }
}
