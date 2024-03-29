using WinterEngine.SceneSystem.Attributes;

namespace WinterEngine.SceneSystem;

// Not bound to any "entity"
public abstract class Entity
{
    public Transform Transform;

    [EntityProperty("globalName")]
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

    public Entity(Transform parent) : this("", parent) { }
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

    #region Component Operations
    public void AddComponent(EntityComponent component)
    {
        
        foreach (var comp in m_Components)
        {
            if (comp.GetType() == component.GetType())
            {
                // it already exists, no duplicate types
                return;
            }
        }

        m_Components.Add(component);
    }

    public T? GetComponent<T>() where T : EntityComponent
    {
        foreach (var component in m_Components)
        {
            if (component is T)
            {
                return (T)component;
            }
        }

        return null;
    }

    public void RemoveComponent<T>() where T : EntityComponent
    {
        var component = m_Components.Find((EntityComponent component) =>
        {
            if (component is T)
            {
                return true;
            }

            return false;
        });

        if (component != null)
        {
            m_Components.Remove(component);
        }
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
}