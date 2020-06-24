using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System.Runtime.CompilerServices;
using ImGuiNET;
using VR1;
using OpenGL;

namespace VR1
{
    class Win : GameWindow
    {
        public Win(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {

        }

        int _vbo, _vao, _ibo;
        private Mesh mesh;
        private Shader shader;
        private ImGuiController _imGuiController;










        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0, 0, 0, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            

            _vbo = GL.GenBuffer();

            shader = new Shader("shaders/shader.v", "shaders/shader.f");



            mesh = MeshLoader.LoadMesh("test.obj");


            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.vertices.Length * Unsafe.SizeOf<Vertex>(), mesh.vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(shader.GetAttributeLocation("aPosition"), 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 0);
            GL.EnableVertexAttribArray(shader.GetAttributeLocation("aPosition"));

            GL.VertexAttribPointer(shader.GetAttributeLocation("aColor"), 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Unsafe.SizeOf<Vector3>());
            GL.EnableVertexAttribArray(shader.GetAttributeLocation("aColor"));

            _ibo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, mesh.indices.Length * sizeof(uint), mesh.indices, BufferUsageHint.StaticDraw);


            _imGuiController = new ImGuiController();

            base.OnLoad(e);
        }
        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(_vbo);

            shader.Dispose();

            base.OnUnload(e);
        }

        private float _scale = 0.5f;
        private float _angle = 0.0f;
        private float _angleZ = 0.0f;
        private float _distanse = 1.0f;
        private bool _persp = false;





        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _imGuiController.NewFrame(this);

            GL.Enable(EnableCap.Blend);

            shader.Use();

            ImGui.Begin("Uniforms");
            ImGui.SliderFloat("Scale", ref _scale, 0, 1);
            ImGui.SliderFloat("Angle", ref _angle, -3.14f, 3.14f);
            ImGui.SliderFloat("Angle on Z", ref _angleZ, -3.14f, 3.14f);
            ImGui.SliderFloat("Distance", ref _distanse, 1, 15.0f);
            ImGui.Checkbox("Perspective View", ref _persp);
            ImGui.End();

            shader.SetUniform("scale", _scale);

            
            var model = Matrix4.CreateRotationY(_angle) * Matrix4.CreateRotationX(_angleZ);
            model = model * Matrix4.CreateTranslation(0, 0, -_distanse);
            shader.SetUniform("my_model", model);
            

            
            if (_persp)
            {
                var perspective_proj = Matrix4.CreatePerspectiveFieldOfView((float)(Math.PI / 2), (float)Width / Height, 0.1f, 100.0f);
                shader.SetUniform("proj", perspective_proj);
            }
            else
            {
                var ortographic_proj = Matrix4.CreateOrthographic(10, 10, -10, 10);
                shader.SetUniform("proj", ortographic_proj);
            }




            GL.PointSize(50);
            GL.DrawElements(PrimitiveType.Triangles, mesh.indices.Length, DrawElementsType.UnsignedInt, 0);

            _imGuiController.Render();

            Context.SwapBuffers();


            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _imGuiController.SetWindowSize(Width, Height);

            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {

            KeyboardState input = Keyboard.GetState();
            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }


            base.OnUpdateFrame(e);
        }

    }
}
