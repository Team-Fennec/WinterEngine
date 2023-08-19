#version 330 core
out vec4 FragColor;

uniform vec4 ourColor; // we set this variable in the OpenGL code.

void main()
{
	FragColor = vec4(1.0, 0.5, 0.5, 1.0);
}