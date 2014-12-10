using SharpDX;

namespace CityScape2
{
    public struct VertexPosNormalTexture
    {
        public VertexPosNormalTexture(Vector3 pos, Vector3 norm, Vector2 tex)
        {
            Position = pos;
            Normal = norm;
            Tex = tex;
        }

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Tex;
    }
}