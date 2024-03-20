using System.Numerics;

namespace WinterEngine.SceneSystem;

// Not bound to any "entity"
public abstract class Entity {
    public Transform Transform;
    public string Name;
    public GUID Guid => m_Guid;
    
    private GUID m_Guid;
    private List<EntityComponent> m_Components;
    
    public Entity()
    {
        m_Guid = Guid.NewGuid();
        m_Components = new List<EntityComponent>();
        
        // add transform component
        
    }

    public virtual void Spawn()
    {
        
    }
    
    public virtual void Death()
    {
        
    }
    
    public virtual void Think(double deltaTime)
    {
        foreach (EntityComponent comp in m_Components)
        {
            comp.Update(deltaTime);
        }
    }
    
    public virtual void Render(double deltaTime)
    {
        foreach (EntityComponent comp in m_Components)
        {
            comp.Render(deltaTime);
        }
    }
}