using System.Numerics;

namespace WinterEngine.SceneSystem;

public class Transform
{
    public Transform Parent => m_Parent;
    private Transform m_Parent;

    public Vector3 Position
    {
        get
        {
            if (m_Parent != null)
                return (LocalPosition + m_Parent.Position);
            else
                return (LocalPosition);
        }
    }
    public Vector3 EulerRotation
    {
        get
        {
            if (m_Parent != null)
                return (LocalEulerRotation + m_Parent.EulerRotation);
            else
                return (LocalEulerRotation);
        }
    }
    public Quaternion Rotation
    {
        get
        {
            if (m_Parent != null)
                return (LocalRotation + m_Parent.Rotation);
            else
                return (LocalRotation);
        }
    }
    public Vector3 Scale
    {
        get
        {
            if (m_Parent != null)
                return (LocalScale + m_Parent.Scale);
            else
                return (LocalScale);
        }
    }

    public Vector3 LocalPosition = Vector3.Zero;
    public Vector3 LocalEulerRotation = Vector3.Zero;
    public Quaternion LocalRotation;
    public Vector3 LocalScale = Vector3.One;

    public void SetParent(Transform parent)
    {
        m_Parent = parent;
    }

    public Transform() { }
    public Transform(Transform parent)
    {
        m_Parent = parent;
    }
}
