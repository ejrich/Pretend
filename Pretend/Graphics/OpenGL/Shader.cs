using System.IO;
using OpenToolkit.Graphics.OpenGL4;

namespace Pretend.Graphics.OpenGL
{
    public class Shader : IShader
    {
        private int _id;

        public Shader(string vertexShader, string fragmentShader)
        {
            var vertexSource = File.ReadAllText(vertexShader);
            var fragmentSource = File.ReadAllText(fragmentShader);
            Compile(vertexSource, fragmentSource);
        }

        ~Shader()
        {
            GL.DeleteProgram(_id);
        }

        public void Bind()
        {
            GL.UseProgram(_id);
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }

        private void Compile(string vertexSource, string fragmentSource)
        {
            _id = GL.CreateProgram();

            var vertexShader = CreateShader(vertexSource, ShaderType.VertexShader);
            if (vertexShader == null) return; // TODO Figure out how to handle this

            var fragmentShader = CreateShader(fragmentSource, ShaderType.FragmentShader);
            if (fragmentShader == null) return; // TODO Figure out how to handle this

            GL.AttachShader(_id, vertexShader.Value);
            GL.AttachShader(_id, fragmentShader.Value);

            // Link our program
            GL.LinkProgram(_id);

            // Note the different functions here: glGetProgram* instead of glGetShader*.
            // GLint isLinked = 0;
            // glGetProgramiv(program, GL_LINK_STATUS, (int *)&isLinked);
            // if (isLinked == GL_FALSE)
            // {
            //     GLint maxLength = 0;
            //     glGetProgramiv(program, GL_INFO_LOG_LENGTH, &maxLength);

            //     // The maxLength includes the NULL character
            //     std::vector<GLchar> infoLog(maxLength);
            //     glGetProgramInfoLog(program, maxLength, &maxLength, &infoLog[0]);
                
            //     // We don't need the program anymore.
            //     glDeleteProgram(program);
            //     // Don't leak shaders either.
            //     glDeleteShader(vertexShader);
            //     glDeleteShader(fragmentShader);
            //     return;
            // }

            // Always detach shaders after a successful link.
            GL.DetachShader(_id, vertexShader.Value);
            GL.DetachShader(_id, fragmentShader.Value);
            GL.DeleteShader(vertexShader.Value);
            GL.DeleteShader(fragmentShader.Value);
        }

        private int? CreateShader(string source, ShaderType shaderType)
        {
            // Create an empty vertex shader handle
            var shader = GL.CreateShader(shaderType);

            // Send the vertex shader source code to GL
            GL.ShaderSource(shader, 1, new [] { source }, new [] { 0 });

            // Compile the vertex shader
            GL.CompileShader(shader);

            // GLint isCompiled = 0;
            var parameters = new int[1];
            GL.GetShader(shader, ShaderParameter.CompileStatus, parameters);
            if(parameters[0] == (int) All.False)
            {
                GL.DeleteShader(shader);
                return null;
            }

            return shader;
        }
    }
}
