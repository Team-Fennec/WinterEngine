namespace WinterEngine.SceneSystem;

public class Transform
{
    public Transform Parent => m_Parent;
    private Transform m_Parent;
    
    // duplicate these for local
    public Vector3 Position;
    public Vector3 EulerRotation;
    public Quaternion Rotation;
    public Vector3 Scale;
    
    public Vector3 LocalPosition;
    public Vector3 LocalEulerRotation;
    public Quaternion LocalRotation;
    public Vector3 LocalScale;
    
    public void SetParent(Transform parent)
    {
        
    }
    
    public Transform(Transform parent)
    {
        m_Parent = parent;
    }
}
