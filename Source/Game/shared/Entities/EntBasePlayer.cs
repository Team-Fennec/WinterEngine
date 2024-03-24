using WinterEngine.SceneSystem;
using WinterEngine.SceneSystem.Attributes;
using WinterEngine.RenderSystem;

namespace WinterEngine.Entities;

// this name is bound to this class, any entities with 
// this name in level data will be deserialized as this.
[EntityClass("ent_player")]
public class EntBasePlayer : EntBaseAnimating
{
}