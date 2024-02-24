﻿using System;
using Veldrid;
using System.Numerics;

namespace WinterEngine.Rendering;

public struct VertexPositionColor {
    public Vector2 Position; // This is the position, in normalized device coordinates.
    public RgbaFloat Color; // This is the color of the vertex.
    public VertexPositionColor(Vector2 position, RgbaFloat color) {
        Position = position;
        Color = color;
    }
    public const uint SizeInBytes = 24;
}
