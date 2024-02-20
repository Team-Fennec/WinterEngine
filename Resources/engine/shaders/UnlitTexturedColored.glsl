VERTEX:
#version 330
layout (location = 0) in vec2 Position;
layout (location = 1) in vec2 UV;
layout (location = 2) in vec4 Color;

uniform mat4 ProjMtx;

out vec2 Frag_UV;
out vec4 Frag_Color;

void main() {
    Frag_UV = UV;
    Frag_Color = Color;
    gl_Position = ProjMtx * vec4(Position, 0, 1);
}

FRAGMENT:
#version 330
uniform sampler2D Texture;

in vec2 Frag_UV;
in vec4 Frag_Color;

layout (location = 0) out vec4 Out_Color;

void main() {
    Out_Color = Frag_Color * texture(Texture, Frag_UV.st);
}
