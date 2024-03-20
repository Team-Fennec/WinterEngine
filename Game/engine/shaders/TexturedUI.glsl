#version 450

#cull_mode FaceCullMode.None
#depth_clip false

VERTEX:
layout(location = 0) in vec2 Position;
layout(location = 1) in vec4 Color;
layout(location = 2) in vec2 TexCoords;

layout(location = 0) out vec4 fsin_Color;
layout(location = 1) out vec2 fsin_texCoords;

void main()
{
    gl_Position = vec4(Position, 0, 1);
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
