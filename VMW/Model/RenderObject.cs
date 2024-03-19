using System;
using OpenTK.Graphics.OpenGL;

namespace VMW.Model
{
    public class RenderObject : IDisposable
    {
        private bool _initialized;

        private readonly int _VAO;
        private readonly int _VBO;
        private readonly int _IBO;

        private readonly Vertex[] _vertices;
        private readonly uint[] _vertexIndeces;


        /// <summary>
        /// Инициализация буфера и массива вершин
        /// </summary>
        /// <param name="vertices"></param>
        public RenderObject(Vertex[] vertices, uint[] vertexIndeces)
        {

            _VBO = GL.GenBuffer();
            _VAO = GL.GenVertexArray();
            _IBO = GL.GenBuffer();

            _vertices = vertices;
            _vertexIndeces = vertexIndeces;

            RenderObjectEBO();

            _initialized = true;
        }

        public  void RenderObjectEBO()
        {
            GL.BindVertexArray(_VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertex.Size * _vertices.Length, _vertices, BufferUsageHint.StreamDraw);
         
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Size, 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, Vertex.Size, sizeof(float) * 3);
            GL.EnableVertexAttribArray(1);



            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _vertexIndeces.Length * sizeof(uint), _vertexIndeces, BufferUsageHint.StreamDraw);
        }

        public void Render()
        {           
            GL.DrawElements(PrimitiveType.Triangles, _vertexIndeces.Length, DrawElementsType.UnsignedInt, 0);     

        }

        /// <summary>
        /// Удаление из массив вершин и удаление буфера
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _initialized)
            {
                GL.DeleteVertexArray(_VAO);
                GL.DeleteBuffer(_VBO);
                _initialized = false;
            }
        }

    }
}
