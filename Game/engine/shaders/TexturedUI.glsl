#version 450

#cull_mode None
#depth_clip false

#include "webase.glsl"

VERTEX:

void main()
{
    gl_Position = vec4(v_Pos, 0, 1);
    fsin_texCoords = v_Uv;
    fsin_Color = v_Col;
}

FRAGMENT:
SHADER_PARAM(0, SurfaceTexture, PARAM_TEXTURE2D);

void main()
{
    fsout_color =  fsin_Color * texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords);
}
