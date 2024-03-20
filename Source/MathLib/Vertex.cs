using System;
using System.Numerics;
using Veldrid;

namespace MathLib;

public struct Vertex {
    public const uint SizeInBytes = 36;

    public float PosX;
    public float PosY;
    public float PosZ;

    public float ColR;
    public float ColG;
    public float ColB;
    public float ColA;

    public float TexU;
    public float TexV;

#region Constructors
    public Vertex() : this(Vector3.Zero, RgbaFloat.White, Vector2.Zero) {}
    public Vertex(Vector3 pos) : this(pos, RgbaFloat.White, Vector2.Zero) {}
    public Vertex(Vector3 pos, RgbaFloat color) : this(pos, color, Vector2.Zero) {}
    public Vertex(Vector3 pos, RgbaFloat color, Vector2 uv) {
        PosX = pos.X;
        PosY = pos.Y;
        PosZ = pos.Z;
        ColR = color.R;
        ColG = color.G;
        ColB = color.B;
        ColA = color.A;
        TexU = uv.X;
        TexV = uv.Y;
    }
#endregion
}
