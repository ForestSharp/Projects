#version 450 core

layout (location = 0) in vec3 position;
layout(location = 1) in vec3 color;
layout (location = 2) in vec3 aNormal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 frag_color;
out vec3 Normal;
out vec3 FragPos;

void main(void)
{
    gl_Position = vec4(position, 1.0) * model * view * projection;
    FragPos = vec3(vec4(position, 1.0) * model);
    Normal =aNormal * mat3(transpose(inverse(model)));
}