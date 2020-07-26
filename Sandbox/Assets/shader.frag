#version 330

in vec2 texCoord;

out vec4 outputColor;

uniform vec4 color;
uniform bool hasTexture = false;
uniform sampler2D texture0;

void main()
{
    outputColor = color * (hasTexture ? texture(texture0, texCoord) : vec4(1f, 1f, 1f, 1f));
}
