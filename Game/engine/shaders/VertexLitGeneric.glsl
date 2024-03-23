#version 450

#cull_mode Back
#depth_clip true

VERTEX:
#include "Buffers.glsl"
#include "Lights.glsl"

layout(location = 0) in vec3 Position;
layout(location = 1) in vec3 Normal;
layout(location = 3) in vec4 Color;
layout(location = 4) in vec2 TexCoords;

layout(location = 0) out vec4 fsin_Color;
layout(location = 1) out vec2 fsin_texCoords;

void main()
{
    vec4 worldPosition = World * vec4(Position, 1);
    vec4 viewPosition = View * worldPosition;
    vec4 clipPosition = Projection * viewPosition;
    gl_Position = clipPosition;
    fsin_texCoords = TexCoords;
    fsin_Color = Color;
}

FRAGMENT:
layout(location = 0) in vec4 fsin_Color;
layout(location = 1) in vec2 fsin_texCoords;
layout(location = 0) out vec4 fsout_color;

layout(set = 1, binding = 0) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 1) uniform sampler SurfaceSampler;

void main()
{
    fsout_color =  fsin_Color * texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords);
}
