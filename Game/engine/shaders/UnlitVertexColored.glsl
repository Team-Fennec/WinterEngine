#version 450

#cull_mode FaceCullMode.Back
#depth_clip true

VERTEX:
#include "Buffers.glsl"

layout(location = 0) in vec3 Position;
layout(location = 1) in vec4 Color;

layout(location = 0) out vec4 fsin_Color;

void main()
{
    vec4 worldPosition = World * vec4(Position, 1);
    vec4 viewPosition = View * worldPosition;
    vec4 clipPosition = Projection * viewPosition;
    gl_Position = clipPosition;
    fsin_Color = Color;
}

FRAGMENT:
layout(location = 0) in vec4 fsin_Color;
layout(location = 0) out vec4 fsout_color;

void main()
{
    fsout_color =  fsin_Color;
}
