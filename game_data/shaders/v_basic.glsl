#version 330 core
layout (location = 0) in vec3 aPos; // vertex location on position 0
layout (location = 1) in vec3 aColor; // vertex color on position 1

out vec3 vertexColor; // output a color to the fragment shader

void main()
{
	gl_Position = vec4(aPos, 1.0);
	vertexColor = aColor;
}