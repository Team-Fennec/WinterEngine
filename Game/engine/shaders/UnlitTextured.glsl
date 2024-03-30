#version 450

#cull_mode Back
#depth_clip true

#include "webase.glsl"

VERTEX:
#include "Buffers.glsl"

void main()
{
    // joint/weight reference:
    // https://github.khronos.org/glTF-Tutorials/gltfTutorial/gltfTutorial_020_Skins.html
    /*mat4 skinMat =
        v_Weight.x * Joints[int(v_Joint.x)] +
        v_Weight.y * Joints[int(v_Joint.y)] +
        v_Weight.z * Joints[int(v_Joint.z)] +
        v_Weight.w * Joints[int(v_Joint.w)];

    vec4 worldPosition = World * skinMat * vec4(v_Pos, 1);*/
    vec4 worldPosition = World * vec4(v_Pos, 1);
    vec4 viewPosition = View * worldPosition;
    gl_Position = Projection * viewPosition;
    fsin_texCoords = v_Uv;
    fsin_Color = v_Col;
}

FRAGMENT:
SHADER_PARAM(0, SurfaceTexture, PARAM_TEXTURE2D);

void main()
{
    fsout_color =  fsin_Color * texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords);
}