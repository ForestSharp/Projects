using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;


namespace VMW.Model
{
    internal class GraphicCore
    {
        private int _program;
        private List<RenderObject> _renderObject = new();

        private Camera _camera;
        private Vector2 _lastPos;

        private Shader _lightingShader;
        private Shader _lampShader;

        private System.Windows.Point _mousePosition;

        internal void Initialization(Size size)
        {
            _lightingShader = new Shader("Shaders\\vertex.vert", "Shaders\\lighting.frag");
            _lampShader = new Shader("Shaders\\vertex.vert", "Shaders\\shader.frag");

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);        

            GL.PatchParameter(PatchParameterInt.PatchVertices, 3);
            GL.Enable(EnableCap.DepthTest);

            _camera = new Camera(Vector3.UnitZ * 100, (float)size.Width / (float)size.Height);
        }

        internal void ToRender()
        {

            GL.ClearColor(Color4.DimGray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 model = Matrix4.Identity;

            _lightingShader.Use();

            _lightingShader.SetMatrix4("model", model);
            _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _lightingShader.SetVector3("material.ambient", new Vector3(1.0f, 0.0f, 0.0f));
            _lightingShader.SetVector3("material.diffuse", new Vector3(1.0f, 0.0f, 0.0f));
            _lightingShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            _lightingShader.SetFloat("material.shininess", 32.0f);

            _lightingShader.SetVector3("light.ambient", new Vector3(0.2f, 0.2f, 0.2f));
            _lightingShader.SetVector3("light.diffuse", new Vector3(0.8f, 0.8f, 0.8f)); // darken the light a bit to fit the scene
            _lightingShader.SetVector3("light.specular", new Vector3(1.0f, 1.0f, 1.0f));

            _lightingShader.SetVector3("lightPos", _camera.Position);
            _lightingShader.SetVector3("viewPos", _camera.Position);

            _lampShader.Use();

            Matrix4 lampMatrix =  Matrix4.CreateTranslation(_camera.Position);
            _lampShader.SetMatrix4("model", lampMatrix);
            _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());


            foreach (var obj in _renderObject)
                obj.Render();

            GL.End();
        }

        internal void LoadRenderObject(Vertex[] vertex, uint[] vertexIndeces)
        {
            RenderObject renderObject = new(vertex, vertexIndeces);
            _renderObject.Add(renderObject);
        }

        internal void ClearRenderObject()
        {
            _renderObject.Clear();
        }

        internal void UpdateProjection(SizeChangedEventArgs changedSize)
        {

        }

        internal void CloseProgram()
        {
            foreach (var obj in _renderObject)
                obj.Dispose();

            GL.DeleteProgram(_program);
        }

        #region Управление камерой

        internal void MoveMouse(Vector2 newMousePosition, System.Windows.Input.MouseEventArgs mouse)
        {
            if (mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed) 
                CameraRepositioning(newMousePosition);


            if (mouse.RightButton == System.Windows.Input.MouseButtonState.Pressed) 
                CameraRotation(newMousePosition);

            _lastPos = new Vector2((float)newMousePosition.X, (float)newMousePosition.Y);
        }

        /// <summary>
        /// Изменение поворота камеры
        /// </summary>
        /// <param name="newMousePosition"></param>
        internal void CameraRotation(Vector2 newMousePosition)
        {
            var deltaX = newMousePosition.X - _lastPos.X;
            var deltaY = newMousePosition.Y - _lastPos.Y;
            _lastPos = new Vector2(newMousePosition.X, newMousePosition.Y);

            const float sensitivity = 0.2f;

            _camera.Yaw += deltaX * sensitivity;
            _camera.Pitch -= deltaY * sensitivity;
        }

        /// <summary>
        /// Изменение позиции камеры
        /// </summary>
        /// <param name="newMousePosition">Новая позиция мыши</param>
        internal void CameraRepositioning(Vector2 newMousePosition)
        {
            var deltaX = newMousePosition.X - _lastPos.X;
            var deltaY = newMousePosition.Y - _lastPos.Y;
            const float cameraSpeed = 0.5f;
            if (deltaX > 0)
                _camera.Position -= _camera.Right * cameraSpeed;
            else if(deltaX < 0)
                _camera.Position += _camera.Right * cameraSpeed;

            if (deltaY > 0)
                _camera.Position -= _camera.Front * cameraSpeed;
            else if(deltaY < 0)
                _camera.Position += _camera.Front * cameraSpeed;
        }

        internal void CameraZoom(int delta)
        {
            var i = delta / 120;
            _camera.Fov -= i;
        }
        #endregion
    }       
}
