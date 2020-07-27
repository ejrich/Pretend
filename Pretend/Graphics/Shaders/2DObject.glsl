#Region Vertex
#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;

out vec2 texCoord;

uniform mat4 transform;
uniform mat4 viewProjection;

void main(void)
{
    texCoord = aTexCoord;
    gl_Position = vec4(aPosition, 1.0) * transform * viewProjection;
}

#Region Fragment
#version 330

in vec2 texCoord;

out vec4 outputColor;

uniform vec4 color;
uniform bool hasTexture = false;
uniform sampler2D texture0;

void main()
{
    outputColor = color;
    if (hasTexture) outputColor *= texture(texture0, texCoord);
}
