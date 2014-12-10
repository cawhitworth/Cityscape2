using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace CityScape2
{
    class Box : IGeometry
    {
        private readonly ushort[] m_Indices;
        private readonly VertexPosNormalTexture[] m_Vertices;

        public Box(Vector3 c1, Vector3 c2)
        {
            var front = new Panel(c1, new Vector2(c2.X - c1.X, c2.Y - c1.Y), Panel.Plane.XY, Panel.Facing.Out);
            var back = new Panel(c2, new Vector2(c1.X - c2.X, c1.Y - c2.Y), Panel.Plane.XY, Panel.Facing.In);
            var right = new Panel(new Vector3(c2.X, c1.Y, c1.Z), new Vector2(c2.Z - c1.Z, c2.Y - c1.Y), Panel.Plane.YZ,
                Panel.Facing.Out);
            var left = new Panel(new Vector3(c1.X, c2.Y, c2.Z), new Vector2(c1.Z - c2.Z, c1.Y - c2.Y), Panel.Plane.YZ,
                Panel.Facing.In);
            var top = new Panel(new Vector3(c1.X, c2.Y, c1.Z), new Vector2(c2.X - c1.X, c2.Z - c1.X), Panel.Plane.XZ,
                Panel.Facing.Out);
            var bottom = new Panel(new Vector3(c2.X, c1.Y, c2.Z), new Vector2(c1.X - c2.X, c1.Z - c2.X), Panel.Plane.XZ,
                Panel.Facing.In);

            var allIndices = new List<ushort>();
            var indices = new [] {front.Indices, back.Indices, right.Indices, left.Indices, top.Indices, bottom.Indices};
            var baseIndex = (ushort) 0;
            foreach (var indexSet in indices)
            {
                allIndices.AddRange( indexSet.Select(i => (ushort)(i + baseIndex)));
                baseIndex += 4;
            }

            var vertices = new[] {front.Vertices, back.Vertices, right.Vertices, left.Vertices, top.Vertices, bottom.Vertices};
            var allVertices = new List<VertexPosNormalTexture>();
            foreach (var vertSet in vertices)
            {
                allVertices.AddRange(vertSet);
            }

            m_Indices = allIndices.ToArray();
            m_Vertices = allVertices.ToArray();
        }


        public IEnumerable<ushort> Indices
        {
            get { return m_Indices; }
        }

        public IEnumerable<VertexPosNormalTexture> Vertices
        {
            get { return m_Vertices; }
        }
    }
}