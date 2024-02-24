using System;
using Veldrid;
using System.Numerics;

namespace WinterEngine.Rendering;

public struct VertexPositionColor
{
    public Vector2 Position; // This is the position, in normalized device coordinates.
    public RgbaFloat Color; // This is the color of the vertex.
    public VertexPositionColor(Vector2 position, RgbaFloat color)
    {
        Position = position;
        Color = color;
    }
    public const uint SizeInBytes = 24;
}

public struct VertexPositionColorTexture
{
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

    public VertexPositionColorTexture(Vector3 pos, RgbaFloat color, Vector2 uv)
    {
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
}

public struct VertexPositionTexture
{
    public const uint SizeInBytes = 20;

    public float PosX;
    public float PosY;
    public float PosZ;

    public float TexU;
    public float TexV;

    public VertexPositionTexture(Vector3 pos, Vector2 uv)
    {
        PosX = pos.X;
        PosY = pos.Y;
        PosZ = pos.Z;
        TexU = uv.X;
        TexV = uv.Y;
    }
}
