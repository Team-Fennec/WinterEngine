using System.Numerics;

namespace WinterEngine.SceneSystem;

// Not bound to any "entity"
public abstract class Entity {
    public Transform Transform;
    public string Name
    {
        get
        {
            if (m_Name == "")
                return m_Guid.ToString();
            else
                return m_Name;
        }
        set
        {
            m_Name = value;
        }
    }   
    public Guid Guid => m_Guid;
    
    private string m_Name;
    private Guid m_Guid;
    private List<EntityComponent> m_Components;
    
    #region Constructors
    public Entity()
    {
        m_Name = "";
        m_Guid = Guid.NewGuid();
        m_Components = new List<EntityComponent>();
        
        // add transform
        this.Transform = new Transform();

        Spawn();
    }

    public Entity(Transform parent) : this("", parent) {}
    public Entity(string name, Transform parent)
    {
        m_Name = "";
        m_Guid = Guid.NewGuid();
        m_Components = new List<EntityComponent>();
        
        // add transform
        this.Transform = new Transform(parent);

        Spawn();
    }
    #endregion

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