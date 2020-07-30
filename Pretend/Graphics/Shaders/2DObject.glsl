#Region Vertex
#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
layout(location = 2) in vec4 aColor;

out vec2 texCoord;
out vec4 color;

uniform mat4 viewProjection;

void main(void)
{
    texCoord = aTexCoord;
    color = aColor;
    gl_Position = vec4(aPosition, 1.0) * viewProjection;
}

#Region Fragment
#version 330

in vec2 texCoord;
in vec4 color;

out vec4 outputColor;

uniform bool hasTexture = false;
uniform sampler2D texture0;

void main()
{
    outputColor = color;
    if (hasTexture) outputColor *= texture(texture0, texCoord);
}
