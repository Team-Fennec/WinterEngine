using System;
using System.Numerics;

namespace WinterEngine.SceneSystem;

public abstract class EntityComponent
{
    public Entity entity => m_Entity;
    private Entity m_Entity;
    
    internal void SetParented(Entity entity)
    {
        m_Entity = ent;
    }

    public virtual void Awake() { }
    public virtual void Death() { }

    public virtual void OnEnable() { }

    public virtual void Update(double deltaTime) { }
    public virtual void FixedUpdate() { }
}
