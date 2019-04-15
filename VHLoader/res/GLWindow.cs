using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Threading;

namespace ConsoleApp9
{
    public sealed class GLWindow : GameWindow
    {
        
        public float[] grid = {1.0f, 0, 100.0f, -1.0f, 0, 100.0f,
                                1.0f, 0, 75.0f, -1.0f, 0, 75.0f,
                                1.0f, 0, 50.0f, -1.0f, 0, 50.0f,
                                1.0f, 0, 25.0f, -1.0f, 0, 25.0f,
                                1.0f, 0, 0, -1.0f, 0, 0,
                                1.0f, 0, -100.0f, -1.0f, 0, -100.0f,
                                1.0f, 0, -75.0f, -1.0f, 0, -75.0f,
                                1.0f, 0, -50.0f, -1.0f, 0, -50.0f,
                                1.0f, 0, -25.0f, -1.0f, 0, -25.0f,
                                1.0f, 0, -100.0f, 1.0f, 0, 100.0f,
                                0.75f, 0, 100.0f, 0.75f, 0, -100.0f,
                                0.50f, 0, 100.0f, 0.50f, 0, -100.0f,
                                0.25f, 0, 100.0f, 0.25f, 0, -100.0f,
                                -1.0f, 0, 100.0f, -1.0f, 0, -100.0f,
                                -0.75f, 0, 100.0f, -0.75f, 0, -100.0f,
                                -0.50f, 0, 100.0f, -0.50f, 0, -100.0f,
                                -0.25f, 0, 100.0f, -0.25f, 0, -100.0f,
                                0, 0, 100.0f, 0, 0, -100.0f };

        public Matrix4 project = Matrix4.CreatePerspectiveFieldOfView((45.0f * 3.14f / 180.0f), 1024.0f / 720.0f, 0.1f, 100.0f);
        public Matrix4 model = new Matrix4()
        {
            Row0 = new OpenTK.Vector4(1, 0, 0, 0),
            Row1 = new OpenTK.Vector4(0, 1, 0, 0),
            Row2 = new OpenTK.Vector4(0, 0, 1, 0),
            Row3 = new OpenTK.Vector4(0, 0, 0, 1),
        };
        public Matrix4 view = new Matrix4()
        {
            Row0 = new OpenTK.Vector4(1, 0, 0, 0),
            Row1 = new OpenTK.Vector4(0, 1, 0, 0),
            Row2 = new OpenTK.Vector4(0, 0, 1, 0),
            Row3 = new OpenTK.Vector4(0, 0, 0, 1),
        };

        private List<RenderObject> _renderObjects = new List<RenderObject>();

        public struct Vertex
        {
            public const int size = 2 * 3 * 4;
            private readonly Vector3 _positions;
            private readonly Vector3 _color;
            public Vertex(Vector3 position, Vector3 color)
            {
                _color = color;
                _positions = position;
                _positions.X = _positions.X / 1024*2 ;
                _positions.Y = _positions.Y / 720*2 ;
                _positions.Z = ((_positions.Z / 720.0f)*2);
            }
            
        }
       
        public GLWindow()
        : base(1024, 720, OpenTK.Graphics.GraphicsMode.Default, "Motion")
        {

        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
        }

        
        private int _program;
        private int programGrid;
        private int uniModel;
        private int uniProj;
        private int uniView;
        private int uniModel2;
        private int uniProj2;
        private int uniView2;

        protected override void OnLoad(EventArgs e)
        {
           view = Matrix4.LookAt(new Vector3(0.0f, 1.0f, 2.0f),
                                  new Vector3(0.0f, 0.0f, 0.0f),
                                  new Vector3(0.0f, 1.0f, 0.0f));

            GL.Enable(EnableCap.DepthTest);

                //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //Matrix4 matrix = Matrix4.CreatePerspectiveFieldOfView(90.0f * 3.14f / 180.0f, 1024 / 720, 1.0f, 1000.0f);
            //GL.LoadMatrix(ref matrix);
            GL.MatrixMode(MatrixMode.Modelview);

            Vertex[] vertices =
            {
                new Vertex(new Vector3(-0.25f, 0.25f, 0.5f), new Vector3(1.0f, 1.0f, 1.0f)),
               new Vertex(new Vector3( 0.0f, -0.25f, 0.5f), new Vector3(1.0f, 1.0f, 1.0f)),
                new Vertex(new Vector3( 0.25f, 0.25f, 0.5f), new Vector3(1.0f, 1.0f, 1.0f)),
                new Vertex(new Vector3(0.6f, 0.7f, 0.5f), new Vector3(1.0f, 1.0f, 1.0f)),
                new Vertex(new Vector3( 0.35f, -0.50f, 0.5f), new Vector3(1.0f, 1.0f, 1.0f)),
               new Vertex(new Vector3( 0.30f, 0.40f, 0.5f), new Vector3(1.0f, 1.0f, 1.0f))
            };
            _renderObjects.Add(new RenderObject(vertices));


            CursorVisible = true;

            //if (StaticGetter.control == 0)
            {
                _program = initShaderProgram("fragmentShader.vert");
                programGrid = initShaderProgram("fragmentShaderGrid.vert");
                uniModel = GL.GetUniformLocation(_program, "model");
                uniProj = GL.GetUniformLocation(_program, "proj");
                uniView = GL.GetUniformLocation(_program, "view");
                uniModel2 = GL.GetUniformLocation(programGrid, "model");
                uniProj2 = GL.GetUniformLocation(programGrid, "proj");
                uniView2 = GL.GetUniformLocation(programGrid, "view");
            }
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 2);
            Closed += OnClosed;

        }

        private void OnClosed(object sender, EventArgs e)
        {
            Exit();
        }
        
        public override void Exit()
        {
            Console.WriteLine("\nExit called");
        //      foreach (var obj in _renderObjects)
        //         obj.Dispose();
         //    GL.DeleteProgram(_program);
         //    GL.DeleteProgram(programGrid);
            _frame = 0;
            //_control++;
            base.Exit();
            Console.WriteLine("\ninput 1 for Skeleton\n2 for hierarchy info\n3 for motion data\n4 for hierarchy- parents\n5 for transformation matrices\n6 show computed XYZ position\n7 write positions to file\n8 for simulation");
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {

        }
        private void HandleKeyboard()
        {
            var keyState = OpenTK.Input.Keyboard.GetState();

            if (keyState.IsKeyDown(OpenTK.Input.Key.Escape))
            {
                Exit();
            }
            else if (keyState.IsKeyDown(OpenTK.Input.Key.Space))
            {   
                //while(OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.Space))
            //    Thread.Sleep(10000);
            }
        }
        double _time;
        int _frame = 0;
        double rotY = 0, rotX = 0;
        double scale = 1;
        bool plane = false;
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            view = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 3.0f),
                       new Vector3(0,0,0),
                       new Vector3(0.0f, 1.0f, 0.0f));

            Matrix4.CreateScale((float)scale, out model);
            //model *= Matrix4.CreateTranslation((float)StaticGetter.file.nodes[1].posXYZ[_frame, 0]/1024, (float)StaticGetter.file.nodes[1].posXYZ[_frame, 1]/720, (float)StaticGetter.file.nodes[1].posXYZ[_frame, 2]/720);
            model *= Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), ((float)rotY * 3.14f/180.0f));
            model *= Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), ((float)rotX * 3.14f / 180.0f));
            //model = Matrix4.Invert(model);
            GL.UniformMatrix4(uniModel, false, ref model);
            GL.UniformMatrix4(uniProj, false, ref project);
            GL.UniformMatrix4(uniView, false, ref view);
            GL.UniformMatrix4(uniModel2, false, ref model);
            GL.UniformMatrix4(uniProj2, false, ref project);
            GL.UniformMatrix4(uniView2, false, ref view);
            _frame++;
            if (_frame >= StaticGetter.file.nodes[1].frameCount)
                _frame = 0;
            _time += e.Time;
            Title = $"(Vsync: {VSync}) FPS: {1f / e.Time:0}";
            Color4 backColor;
            backColor.A = 1.0f;
            backColor.R = 0.1f;
            backColor.G = 0.1f;
            backColor.B = 0.3f;
            GL.ClearColor(backColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Vertex[] vertices = new Vertex[StaticGetter.file.nodes.Count*2 + 40];
            int pointCount = 2, lineCount = 2; 
            for (int i = 1, j=0; i < StaticGetter.file.nodes.Count  ; i++, j+=2)
            {
                Vector3 tempPos = new Vector3 ( (float)StaticGetter.file.nodes[i].posXYZ[_frame,0], (float)StaticGetter.file.nodes[i].posXYZ[_frame,1], (float)StaticGetter.file.nodes[i].posXYZ[_frame, 2]);
                Vector3 tempCol = new Vector3(1.0f, 0.0f, 0.0f);
                vertices[j] = new Vertex(tempPos, tempCol);
                if  (StaticGetter.file.nodes[i].getParent() == 0) //min parent
                    tempPos = new Vector3((float)StaticGetter.file.nodes[1].posXYZ[_frame, 0], (float)StaticGetter.file.nodes[1].posXYZ[_frame, 1], (float)StaticGetter.file.nodes[1].posXYZ[_frame, 2]);
                else 
                    tempPos = new Vector3((float)StaticGetter.file.nodes[StaticGetter.file.nodes[i].getParent()].posXYZ[_frame, 0], (float)StaticGetter.file.nodes[StaticGetter.file.nodes[i].getParent()].posXYZ[_frame, 1], (float)StaticGetter.file.nodes[StaticGetter.file.nodes[i].getParent()].posXYZ[_frame, 2]);
                vertices[j+1] = new Vertex(tempPos, tempCol);
                pointCount+=2;
            }
            //vertices[0] = new Vertex(new Vector3((float)StaticGetter.file.nodes[1].posXYZ[_frame, 0], (float)StaticGetter.file.nodes[1].posXYZ[_frame, 1], (float)StaticGetter.file.nodes[1].posXYZ[_frame, 2]), new Vector3(1.0f, 0.0f, 0.0f));
            for (int i = StaticGetter.file.nodes.Count+1, j = StaticGetter.file.nodes.Count * 2; i < StaticGetter.file.nodes.Count + 108; i += 3, j++)
            {
                vertices[j] = new Vertex(new Vector3(grid[i - (StaticGetter.file.nodes.Count + 1)] * 1024, grid[i - (StaticGetter.file.nodes.Count) ] * 720, grid[i - (StaticGetter.file.nodes.Count - 1)] * (7.2f)), new Vector3(1.0f, 0.0f, 1.0f));
                lineCount += 2; 
            }
            vertices[StaticGetter.file.nodes.Count*2 + 36] = new Vertex(new Vector3(-1024.0f, -0.1f, -720.0f), new Vector3(1.0f, 1.0f, 1.0f));
            vertices[StaticGetter.file.nodes.Count * 2 + 37] = new Vertex(new Vector3(1024.0f, -0.1f, -720.0f), new Vector3(1.0f, 1.0f, 1.0f));
            vertices[StaticGetter.file.nodes.Count * 2 + 38] = new Vertex(new Vector3(1024.0f, -0.1f, 720.0f), new Vector3(1.0f, 1.0f, 1.0f));
            vertices[StaticGetter.file.nodes.Count * 2 + 39] = new Vertex(new Vector3(-1024.0f, -0.1f, 720.0f), new Vector3(1.0f, 1.0f, 1.0f));
            _renderObjects[0] = (new RenderObject(vertices));




           //foreach (var renderObject in _renderObjects)
           //{
                _renderObjects[0].Render(StaticGetter.file.nodes.Count*2, _program, plane);
            HandleKeyboard();
            //}

            if (OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.Up))
            {
              scale += 0.05;
            
            }
            if (OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.Down))
            {
                scale -= 0.05;

            }
            if (OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.Left))
            {
                rotY += 2.0;

            }
            if (OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.Right))
            {
                rotY -= 2.0;

            }
            if (OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.S))
            {
                rotX -= 2.0;
            }
            if (OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.W))
            {
                rotX += 2.0;
            }
            if (OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.P))
            {
                Thread.Sleep(100);
                if (plane == false)
                    plane = true;
                else
                    plane = false; 
            }
            //model *= Matrix4.CreateTranslation(-(float)StaticGetter.file.nodes[1].posXYZ[_frame, 0] / 1024, -(float)StaticGetter.file.nodes[1].posXYZ[_frame, 1] / 720, -(float)StaticGetter.file.nodes[1].posXYZ[_frame, 2] / 720);
            SwapBuffers();
        }

        
        //public const string shaderPath = (("($(SolutionDir)") + @"\Shaders");
        private int initShaderProgram( string frag)
        {
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, File.ReadAllText("vertexShader.vert"));
            GL.CompileShader(vertexShader);
            int status;
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out status);
            if (status == 0)
                throw new GraphicsException(
                    String.Format("Error compiling vertex shader: {0}", GL.GetShaderInfoLog(vertexShader)));

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, File.ReadAllText(frag));
            GL.CompileShader(fragmentShader);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out status);
            if (status == 0)
                throw new GraphicsException(
                    String.Format("Error compiling fragment shader: {0}", GL.GetShaderInfoLog(fragmentShader)));

            var program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);

            GL.DetachShader(program, vertexShader);
            GL.DetachShader(program, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            return program;
        }
    }
}
