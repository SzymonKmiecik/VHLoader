using System;
using OpenTK.Graphics.OpenGL;


namespace ConsoleApp9
{
    public class RenderObject : IDisposable
    {
        private bool _initialized;
        private int _vertexArray;
        private int _buffer;
        private int _verticeCount;
        public RenderObject(GLWindow.Vertex[] vertices)
        {
            this._verticeCount = vertices.Length;
            this._vertexArray = GL.GenVertexArray();
            this._buffer = GL.GenBuffer();

            GL.BindVertexArray(this._vertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._vertexArray);

            // create first buffer: vertex
            GL.NamedBufferStorage(
                this._buffer,
                GLWindow.Vertex.size * vertices.Length,        // the size needed by this buffer
                vertices,                           // data to initialize with
                BufferStorageFlags.MapWriteBit);    // at this point we will only write to the buffer


            GL.VertexArrayAttribBinding(this._vertexArray, 0, 0);
            GL.EnableVertexArrayAttrib(this._vertexArray, 0);
            GL.VertexArrayAttribFormat(
                this._vertexArray,
                0,                      // attribute index, from the shader location = 0
                3,                      // size of attribute, vec4
                VertexAttribType.Float, // contains floats
                true,                  // does not need to be normalized as it is already, floats ignore this flag anyway
                0);                     // relative offset, first item


            GL.VertexArrayAttribBinding(this._vertexArray, 1, 0);
            GL.EnableVertexArrayAttrib(this._vertexArray, 1);
            GL.VertexArrayAttribFormat(
                this._vertexArray,
                1,                      // attribute index, from the shader location = 1
                3,                      // size of attribute, vec4
                VertexAttribType.Float, // contains floats
                false,                  // does not need to be normalized as it is already, floats ignore this flag anyway
                12);                     // relative offset after a vec4

            // link the vertex array and buffer and provide the stride as size of Vertex
            GL.VertexArrayVertexBuffer(this._vertexArray, 0, this._buffer, IntPtr.Zero, GLWindow.Vertex.size);
            _initialized = true;
        }
        public void Render(int p, int l, bool plane)
        {
            GL.UseProgram(l);
            GL.BindVertexArray(this._vertexArray);
            GL.LineWidth(3);
            GL.DrawArrays(PrimitiveType.Lines, 0, p);
            GL.DrawArrays(PrimitiveType.Points, 0, p);
            GL.PointSize(7);
            GL.LineWidth(1);
            GL.DrawArrays(PrimitiveType.Lines, p, 36);
            if (plane == true)
            GL.DrawArrays(PrimitiveType.Quads, p+36, 4);
            GL.DeleteVertexArray(this._vertexArray);
            GL.DeleteBuffer(this._buffer);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_initialized)
                {
                    GL.DeleteVertexArray(_vertexArray);
                    GL.DeleteBuffer(this._buffer);
                    _initialized = false;
                }
            }
        }
    }
}