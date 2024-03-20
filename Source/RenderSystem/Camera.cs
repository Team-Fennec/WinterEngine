using MathLib;
using System.Numerics;
using WinterEngine;

namespace WinterEngine.RenderSystem;

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
        
    }
    
    public Matrix4x4 GetView()
    {
        
    }
}
