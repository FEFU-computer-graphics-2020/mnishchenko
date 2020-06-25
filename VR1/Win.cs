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



        // camera
        float speed = 1.5f;
        Vector3 position = new Vector3(0.0f, 0.0f, 3.0f);
        Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
        Matrix4 view = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 3.0f),
             new Vector3(0.0f, 0.0f, 0.0f),
             new Vector3(0.0f, 1.0f, 0.0f));





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
        private float _moveX = 1f;
        private float _moveY = 1f;







        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

           
            
            

            _imGuiController.NewFrame(this);

            GL.Enable(EnableCap.Blend);

            shader.Use();

            ImGui.Begin("Uniforms");
            ImGui.SliderFloat("Move X", ref _moveX, -10, 10);
            ImGui.SliderFloat("Move Y", ref _moveY, -10, 10);
            ImGui.SliderFloat("Scale", ref _scale, 0, 10);
            ImGui.SliderFloat("Angle", ref _angle, -3.14f, 3.14f);
            ImGui.SliderFloat("Angle on Z", ref _angleZ, -3.14f, 3.14f);
            ImGui.SliderFloat("Distance", ref _distanse, 1, 15.0f);
            ImGui.Checkbox("Perspective View", ref _persp);
            ImGui.End();

            ImGui.Begin("Controls");
            ImGui.Text("W - Forward");
            ImGui.Text("S - Backward");
            ImGui.Text("A - Left");
            ImGui.Text("D - Right");
            ImGui.Text("Space - Up");
            ImGui.Text("LCtrl - Down");
            ImGui.End();

            shader.SetUniform("scale", _scale);

            
            var model = Matrix4.CreateRotationY(_angle) * Matrix4.CreateRotationX(_angleZ);
            model = model * Matrix4.CreateTranslation(_moveX, _moveY, -_distanse);
            shader.SetUniform("my_model", model);
            

            
            if (_persp)
            {
                var perspective_proj = Matrix4.CreatePerspectiveFieldOfView((float)(Math.PI / 2), (float)Width / Height, 0.1f, 100.0f);
                shader.SetUniform("proj", perspective_proj);
            }
            else
            {
                var ortographic_proj = Matrix4.CreateOrthographic(10, 10, -10, 10); //.CreateOrthographicOffCenter(0.0f, 800.0f, 0.0f, 600.0f, 0.1f, 100.0f);
                shader.SetUniform("proj", ortographic_proj);
            }

            view = Matrix4.LookAt(position, position + front, up);
            
            
            GL.UniformMatrix4(21, false, ref view);


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


            if (!Focused) // check focused (on window)
            {
                return;
            }

            if (input.IsKeyDown(Key.W))
            {
                Console.WriteLine("MOVE FORWARD");
                position += front * speed * (float)e.Time; //Forward 
            }

            if (input.IsKeyDown(Key.S))
            {
                Console.WriteLine("MOVE BACK");
                position -= front * speed * (float)e.Time; //Backwards
            }

            if (input.IsKeyDown(Key.A))
            {
                Console.WriteLine("LEFT");
                position -= Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)e.Time; //Left
            }

            if (input.IsKeyDown(Key.D))
            {
                Console.WriteLine("RIGHT");
                position += Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)e.Time; //Right
            }

            if (input.IsKeyDown(Key.Space))
            {
                Console.WriteLine("UP");
                position += up * speed * (float)e.Time; //Up 
            }

            if (input.IsKeyDown(Key.LControl))
            {
                Console.WriteLine("DOWN");
                position -= up * speed * (float)e.Time; //Down
            }


            base.OnUpdateFrame(e);
        }

    }
}
