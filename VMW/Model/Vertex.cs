using OpenTK.Mathematics;

namespace VMW.Model
{
    public struct Vertex
    {
        public const int Size = (3 + 3 + 4) * sizeof(float);

        private readonly Vector3 _position;
        private readonly Vector3 _normal;
        private readonly Color4 _color;

        public Vertex(Vector3 position, Color4 color, Vector3 normal = new())
        {
            _position = position;
            _normal = normal;
            _color = color;
        }
    }
}
