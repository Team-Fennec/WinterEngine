/*
	WinterEngine Base Shader Code
	
	Imported by every shader

	(C) 2024 Team Fennec
*/

// this is automatically defined in the vertex shader
#if VERTEX_SHADER
	// if you change this, change the vertex layout as well
	layout(location = 0) in vec3 Position;
	layout(location = 1) in vec3 Normal;
	layout(location = 3) in vec4 Color;
	layout(location = 4) in vec2 TexCoords;
	#define v_Pos Position
	#define v_Norm Normal
	#define v_Col Color
	#define v_Uv TexCoords

	layout(location = 0) out vec4 fsin_Color;
	layout(location = 1) out vec2 fsin_texCoords;
#else
	layout(location = 0) in vec4 fsin_Color;
	layout(location = 1) in vec2 fsin_texCoords;
	layout(location = 0) out vec4 fsout_color;

	layout(set = 0, binding = 3) uniform sampler SurfaceSampler;
#endif

// param types, wrapped just for linguistic convenience
#define PARAM_TEXTURE2D texture2D
#define PARAM_SAMPLER sampler
#define PARAM_INTEGER int
#define PARAM_FLOAT float
#define PARAM_VECTOR2 vec2
#define PARAM_VECTOR3 vec3
#define PARAM_VECTOR4 vec4

#define SHADER_PARAM(id, name, type) layout(set = 1, binding = id) uniform type name
