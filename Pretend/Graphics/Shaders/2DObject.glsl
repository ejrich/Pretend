#Region Vertex
#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
layout(location = 2) in vec4 aColor;
layout(location = 3) in int aTextureIndex;
layout(location = 4) in int aSingleChannel;

out vec2 texCoord;
out vec4 color;
flat out int textureIndex;
flat out int singleChannel;

uniform mat4 viewProjection;

void main(void)
{
    texCoord = aTexCoord;
    color = aColor;
    textureIndex = aTextureIndex;
    singleChannel = aSingleChannel;
    gl_Position = vec4(aPosition, 1.0) * viewProjection;
}

#Region Fragment
#version 330

in vec2 texCoord;
in vec4 color;
flat in int textureIndex;
flat in int singleChannel;

out vec4 outputColor;

uniform sampler2D textures[32];

vec4 determineTextureColor(vec4 textureColor)
{
    if (singleChannel == 1) {
        if (textureColor.r == 0)
            discard;
        return vec4(1.0, 1.0, 1.0, textureColor.r);
    }
    if (textureColor.a < 0.1)
        discard;
    return textureColor;
}

void main()
{
    outputColor = color;
    if (textureIndex >= 0) {
        switch (textureIndex) {
            case 0: outputColor *= determineTextureColor(texture(textures[0], texCoord)); break;
            case 1: outputColor *= determineTextureColor(texture(textures[1], texCoord)); break;
            case 2: outputColor *= determineTextureColor(texture(textures[2], texCoord)); break;
            case 3: outputColor *= determineTextureColor(texture(textures[3], texCoord)); break;
            case 4: outputColor *= determineTextureColor(texture(textures[4], texCoord)); break;
            case 5: outputColor *= determineTextureColor(texture(textures[5], texCoord)); break;
            case 6: outputColor *= determineTextureColor(texture(textures[6], texCoord)); break;
            case 7: outputColor *= determineTextureColor(texture(textures[7], texCoord)); break;
            case 8: outputColor *= determineTextureColor(texture(textures[8], texCoord)); break;
            case 9: outputColor *= determineTextureColor(texture(textures[9], texCoord)); break;
            case 10: outputColor *= determineTextureColor(texture(textures[10], texCoord)); break;
            case 11: outputColor *= determineTextureColor(texture(textures[11], texCoord)); break;
            case 12: outputColor *= determineTextureColor(texture(textures[12], texCoord)); break;
            case 13: outputColor *= determineTextureColor(texture(textures[13], texCoord)); break;
            case 14: outputColor *= determineTextureColor(texture(textures[14], texCoord)); break;
            case 15: outputColor *= determineTextureColor(texture(textures[15], texCoord)); break;
            case 16: outputColor *= determineTextureColor(texture(textures[16], texCoord)); break;
            case 17: outputColor *= determineTextureColor(texture(textures[17], texCoord)); break;
            case 18: outputColor *= determineTextureColor(texture(textures[18], texCoord)); break;
            case 19: outputColor *= determineTextureColor(texture(textures[19], texCoord)); break;
            case 20: outputColor *= determineTextureColor(texture(textures[20], texCoord)); break;
            case 21: outputColor *= determineTextureColor(texture(textures[21], texCoord)); break;
            case 22: outputColor *= determineTextureColor(texture(textures[22], texCoord)); break;
            case 23: outputColor *= determineTextureColor(texture(textures[23], texCoord)); break;
            case 24: outputColor *= determineTextureColor(texture(textures[24], texCoord)); break;
            case 25: outputColor *= determineTextureColor(texture(textures[25], texCoord)); break;
            case 26: outputColor *= determineTextureColor(texture(textures[26], texCoord)); break;
            case 27: outputColor *= determineTextureColor(texture(textures[27], texCoord)); break;
            case 28: outputColor *= determineTextureColor(texture(textures[28], texCoord)); break;
            case 29: outputColor *= determineTextureColor(texture(textures[29], texCoord)); break;
            case 30: outputColor *= determineTextureColor(texture(textures[30], texCoord)); break;
            case 31: outputColor *= determineTextureColor(texture(textures[31], texCoord)); break;
        }
    }
}
