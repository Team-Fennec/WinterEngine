#version 450

#cull_mode Back
#depth_clip true

#include "webase.glsl"

VERTEX:
#include "Buffers.glsl"

void main()
{
    vec4 worldPosition = World * vec4(v_Pos, 1);
    vec4 viewPosition = View * worldPosition;
    vec4 clipPosition = Projection * viewPosition;
    gl_Position = clipPosition;
    fsin_Color = v_Col;
}

FRAGMENT:

void main()
{
    fsout_color = fsin_Color;
}
