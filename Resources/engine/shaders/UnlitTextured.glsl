VERTEX:
#version 450

#include "Buffers.glsl"

layout(location = 0) in vec3 Position;
layout(location = 1) in vec4 Color;
layout(location = 2) in vec2 TexCoords;

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
#version 450

layout(location = 0) in vec4 fsin_Color;
layout(location = 1) in vec2 fsin_texCoords;
layout(location = 0) out vec4 fsout_color;

layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler SurfaceSampler;

void main()
{
    fsout_color =  fsin_Color * texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords);
}