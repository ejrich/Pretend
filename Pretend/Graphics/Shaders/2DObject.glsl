#Region Vertex
#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
layout(location = 2) in vec4 aColor;
layout(location = 3) in int aTextureIndex;

out vec2 texCoord;
out vec4 color;
flat out int textureIndex;

uniform mat4 viewProjection;

void main(void)
{
    texCoord = aTexCoord;
    color = aColor;
    textureIndex = aTextureIndex;
    gl_Position = vec4(aPosition, 1.0) * viewProjection;
}

#Region Fragment
#version 330

in vec2 texCoord;
in vec4 color;
flat in int textureIndex;

out vec4 outputColor;

uniform sampler2D textures[32];

void main()
{
    outputColor = color;
    if (textureIndex >= 0) {
        switch (textureIndex) {
            case 0: outputColor *= texture(textures[0], texCoord); break;
            case 1: outputColor *= texture(textures[1], texCoord); break;
            case 2: outputColor *= texture(textures[2], texCoord); break;
            case 3: outputColor *= texture(textures[3], texCoord); break;
            case 4: outputColor *= texture(textures[4], texCoord); break;
            case 5: outputColor *= texture(textures[5], texCoord); break;
            case 6: outputColor *= texture(textures[6], texCoord); break;
            case 7: outputColor *= texture(textures[7], texCoord); break;
            case 8: outputColor *= texture(textures[8], texCoord); break;
            case 9: outputColor *= texture(textures[9], texCoord); break;
            case 10: outputColor *= texture(textures[10], texCoord); break;
            case 11: outputColor *= texture(textures[11], texCoord); break;
            case 12: outputColor *= texture(textures[12], texCoord); break;
            case 13: outputColor *= texture(textures[13], texCoord); break;
            case 14: outputColor *= texture(textures[14], texCoord); break;
            case 15: outputColor *= texture(textures[15], texCoord); break;
            case 16: outputColor *= texture(textures[16], texCoord); break;
            case 17: outputColor *= texture(textures[17], texCoord); break;
            case 18: outputColor *= texture(textures[18], texCoord); break;
            case 19: outputColor *= texture(textures[19], texCoord); break;
            case 20: outputColor *= texture(textures[20], texCoord); break;
            case 21: outputColor *= texture(textures[21], texCoord); break;
            case 22: outputColor *= texture(textures[22], texCoord); break;
            case 23: outputColor *= texture(textures[23], texCoord); break;
            case 24: outputColor *= texture(textures[24], texCoord); break;
            case 25: outputColor *= texture(textures[25], texCoord); break;
            case 26: outputColor *= texture(textures[26], texCoord); break;
            case 27: outputColor *= texture(textures[27], texCoord); break;
            case 28: outputColor *= texture(textures[28], texCoord); break;
            case 29: outputColor *= texture(textures[29], texCoord); break;
            case 30: outputColor *= texture(textures[30], texCoord); break;
            case 31: outputColor *= texture(textures[31], texCoord); break;
        }
    }
}
