#version 440 core

out vec4 color;
in vec3 colore;
void main(void)
{
color = vec4(colore, 1.0);
}