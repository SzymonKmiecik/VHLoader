#version 440 core

out vec4 color;
in vec3 Color;
void main(void)
{
color = vec4(Color, 1.0);
}