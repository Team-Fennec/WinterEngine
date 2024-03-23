using WinterEngine.SceneSystem;
using WinterEngine.SceneSystem.Attributes;
using WinterEngine.RenderSystem;

namespace WinterEngine.Entities;

// this name is bound to this class, any entities with 
// this name in level data will be deserialized as this.
[EntityClass("prop_dynamic")]
public class EntPropDynamic : EntBaseAnimating
{
    // these properties are set via scene system deserialization
    [EntityProperty("worldModel")]
    public string ModelName;
}
