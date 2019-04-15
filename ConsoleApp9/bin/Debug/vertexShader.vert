#version 450 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 color;
out vec3 Color;
uniform mat4 model;
uniform mat4 proj;
uniform mat4 view;

void main(void)
{
Color = color;
if (position.z < 0.0f)
{
gl_Position =proj * view * model * vec4(position.x,position.y,position.z, 1.0f);
}
else 
{
 gl_Position = proj * view * model * vec4(position, 1.0f);
}
}