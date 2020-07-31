#Region Vertex
#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
layout(location = 2) in vec4 aColor;
layout(location = 3) in int aHasTexture;

out vec2 texCoord;
out vec4 color;
flat out int hasTexture;

uniform mat4 viewProjection;

void main(void)
{
    texCoord = aTexCoord;
    color = aColor;
    hasTexture = aHasTexture;
    gl_Position = vec4(aPosition, 1.0) * viewProjection;
}

#Region Fragment
#version 330

in vec2 texCoord;
in vec4 color;
flat in int hasTexture;

out vec4 outputColor;

uniform sampler2D texture0;

void main()
{
    outputColor = color;
    if (hasTexture == 1) outputColor *= texture(texture0, texCoord);
}
