using SharpDX;

namespace CityScape2.Rendering
{
    public struct VertexPosNormalTextureMod
    {
        public VertexPosNormalTextureMod(Vector3 pos, Vector3 norm, Vector2 tex, Vector3 mod)
        {
            Mod = mod;
            Position = pos;
            Normal = norm;
            Tex = tex;

        }

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Tex;
        public Vector3 Mod;
    }
}