#version 330 core
// Basic uniform color test fragment shader
out vec4 FragColor;

uniform vec4 ourColor; // we set this variable in the OpenGL code.

void main()
{
	FragColor = ourColor;
}