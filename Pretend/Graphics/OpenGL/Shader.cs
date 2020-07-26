using System.Collections.Generic;
using System.IO;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;

namespace Pretend.Graphics.OpenGL
{
    public class Shader : IShader
    {
        private int _id;
        private IDictionary<string, int> _uniforms;

        public Shader()
        {
            _id = GL.CreateProgram();
            _uniforms = new Dictionary<string, int>();
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

        public void Compile(string vertexFile, string fragmentFile)
        {
            var shaders = new List<(ShaderType type, string file)>
            {
                (ShaderType.VertexShader, vertexFile), (ShaderType.FragmentShader, fragmentFile)
            };
            var compiledShaders = new List<int>();

            foreach (var shader in shaders)
            {
                var compiledShader = CreateShader(shader.file, shader.type);
                if (compiledShader == null) return; // TODO Figure out how to handle this

                GL.AttachShader(_id, compiledShader.Value);
                compiledShaders.Add(compiledShader.Value);
            }

            GL.LinkProgram(_id);

            GL.GetProgram(_id, GetProgramParameterName.LinkStatus, out var isLinked);
            if (isLinked == (int) All.False)
            {
                GL.GetProgram(_id, GetProgramParameterName.InfoLogLength, out var maxLength);

                GL.GetProgramInfoLog(_id, maxLength, out var _, out var infoLog);

                // We don't need the program anymore.
                GL.DeleteProgram(_id);
                // Don't leak shaders either.
                foreach (var shader in compiledShaders)
                    GL.DeleteShader(shader);
                return;
            }

            foreach (var shader in compiledShaders)
            {
                GL.DetachShader(_id, shader);
                GL.DeleteShader(shader);
            }

            GL.GetProgram(_id, GetProgramParameterName.ActiveUniforms, out var uniforms);

            for (var i = 0; i < uniforms; i++)
            {
                var name = GL.GetActiveUniform(_id, i, out _, out _);

                var location = GL.GetUniformLocation(_id, name);

                _uniforms.Add(name, location);
            }
        }

        private int? CreateShader(string file, ShaderType shaderType)
        {
            var source = File.ReadAllText(file);
            var shader = GL.CreateShader(shaderType);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out var statusCode);
            if(statusCode == (int) All.False)
            {
                GL.DeleteShader(shader);
                return null;
            }

            return shader;
        }

        public void SetBool(string name, bool value)
        {
            GL.UseProgram(_id);
            GL.Uniform1(_uniforms[name], value ? 1 : 0);
        }

        public void SetInt(string name, int value)
        {
            GL.UseProgram(_id);
            GL.Uniform1(_uniforms[name], value);
        }

        public void SetFloat(string name, float value)
        {
            GL.UseProgram(_id);
            GL.Uniform1(_uniforms[name], value);
        }

        public void SetVec4(string name, Vector4 value)
        {
            GL.UseProgram(_id);
            GL.Uniform4(_uniforms[name], value);
        }

        public void SetMat4(string name, Matrix4 value)
        {
            GL.UseProgram(_id);
            GL.UniformMatrix4(_uniforms[name], true, ref value);
        }
    }
}
