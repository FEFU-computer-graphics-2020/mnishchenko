using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace VR1
{
    class Shader
    {
        int Handle;
        int CompileShader(string path, ShaderType shaderType)
        {
            string shaderSource;

            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                shaderSource = reader.ReadToEnd();
            }

            var shader = GL.CreateShader(shaderType);
            GL.ShaderSource(shader, shaderSource);

            GL.CompileShader(shader);

            var infoLog = GL.GetShaderInfoLog(shader);

            if (infoLog.Length != 0)
            {
                System.Console.WriteLine($"Failed at compile shader: {path}");
                System.Console.WriteLine(infoLog + "\n\nНе судьба\n");
                return 0;
            }

            return shader;
        }

        public Shader(string vertexPath, string fragmentPath)
        {
            var vertexShader = CompileShader(vertexPath, ShaderType.VertexShader);
            var fragmentShader = CompileShader(fragmentPath, ShaderType.FragmentShader);

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);

            GL.LinkProgram(Handle);

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }
    }
}
