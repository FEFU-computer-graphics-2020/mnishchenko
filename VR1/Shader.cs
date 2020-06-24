using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace VR1
{

    class Shader
    {
        int _handle;
        int VertexShader, FragmentShader;

        Dictionary<string, int> _uniforms = new Dictionary<string, int>();
        Dictionary<string, int> _attribs = new Dictionary<string, int>();

        private int ReadyShader(string path, ShaderType shaderType)
        {
            string ShaderSource;
            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                ShaderSource = reader.ReadToEnd();
            }

            var shader = GL.CreateShader(shaderType);
            GL.ShaderSource(shader, ShaderSource);

            GL.CompileShader(shader);

            string infoLogVert = GL.GetShaderInfoLog(shader);
            if (infoLogVert != System.String.Empty)
                Console.WriteLine("Get some errors:\n", infoLogVert);

            return shader;
        }

        public Shader(string vertexPath, string fragmentPath)
        {

            VertexShader = ReadyShader(vertexPath, ShaderType.VertexShader);
            FragmentShader = ReadyShader(fragmentPath, ShaderType.FragmentShader);

            _handle = GL.CreateProgram();

            GL.AttachShader(_handle, VertexShader);
            GL.AttachShader(_handle, FragmentShader);

            GL.LinkProgram(_handle);

            GL.DetachShader(_handle, VertexShader);
            GL.DetachShader(_handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);

        }

        public void Use()
        {
            GL.UseProgram(_handle);
        }


        public int GetAttributeLocation(string name)
        {
            if (!_attribs.ContainsKey(name))
            {
                var location = GL.GetAttribLocation(_handle, name);
                if (location < 0)
                {
                    Console.WriteLine($"В шейдере нет аттрибута - {name}");
                    return -1;
                }

                _attribs.Add(name, location);
            }

            return _attribs[name];
        }


        public int GetUniformLocation(string name)
        {
            if (!_uniforms.ContainsKey(name))
            {
                var location = GL.GetUniformLocation(_handle, name);
                if (location < 0)
                {
                    Console.WriteLine($"В шейдере нет параметра - {name}");
                    return -1;
                }

                _uniforms.Add(name, location);
            }

            return _uniforms[name];
        }


        public void SetUniform(string name, float val)
        {
            GL.Uniform1(GetUniformLocation(name), val);
        }
        public void SetUniform(string name, Matrix4 val)
        {
            GL.UniformMatrix4(GetUniformLocation(name), false, ref val);
        }







        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(_handle);

                disposedValue = true;
            }
        }



        /*~Shader()
        {
            GL.DeleteProgram(_handle);
        }*/


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }









    }
}
