using MathLib;
using System.Numerics;

namespace WinterEngine.SceneSystem;

public class Camera
{
    public enum ProjectionMode
    {
        Orthographic,
        Perspective
    }
    
    public float FOV;
    public Vector3 Position;
    public Vector3 Rotation;
    public ProjectionMode Mode = ProjectionMode.Perspective;
    
    public Matrix4x4 GetProjection()
    {
        throw new NotImplementedException();
    }
    
    public Matrix4x4 GetView()
    {
        throw new NotImplementedException();
    }
}
