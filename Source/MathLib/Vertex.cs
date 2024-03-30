using System;
using System.Numerics;
using Veldrid;

namespace MathLib;

public struct Vertex
{
    public const uint SizeInBytes = 80;

    public Vector3 Position;
    public Vector3 Normal;
    public RgbaFloat Color;
    public Vector2 UV;
    public Vector4 Joint;
    public Vector4 Weight;

    #region Constructors
    public Vertex() : this(Vector3.Zero, Vector3.One, RgbaFloat.White, Vector2.Zero) { }
    public Vertex(Vector3 pos) : this(pos, Vector3.One, RgbaFloat.White, Vector2.Zero) { }
    public Vertex(Vector3 pos, Vector3 normal) : this(pos, normal, RgbaFloat.White, Vector2.Zero) { }
    public Vertex(Vector3 pos, Vector3 normal, RgbaFloat color) : this(pos, normal, color, Vector2.Zero) { }
    public Vertex(Vector3 pos, Vector3 normal, RgbaFloat color, Vector2 uv)
    {
        Position = pos;
        Color = color;
        Normal = normal;
        UV = uv;
    }
    #endregion
}
