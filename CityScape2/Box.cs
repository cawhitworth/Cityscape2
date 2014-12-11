using System.Collections.Generic;
using SharpDX;

namespace CityScape2
{
    class Box : IGeometry
    {
        private readonly AggregateGeometry m_Aggregate;

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

            m_Aggregate = new AggregateGeometry(front, back, right, left, top, bottom);
        }

        public IEnumerable<ushort> Indices
        {
            get { return m_Aggregate.Indices; }
        }

        public IEnumerable<VertexPosNormalTexture> Vertices
        {
            get { return m_Aggregate.Vertices; }
        }
    }
}