using System.Collections.Generic;
using CityScape2.Rendering;
using SharpDX;

namespace CityScape2.Geometry
{
    class Box : IGeometry
    {
        private readonly AggregateGeometry m_Aggregate;

        public Box(Vector3 c1, Vector3 c2)
        {
            Color mod = Color.White;
            var front = new Panel(c1, new Vector2(c2.X - c1.X, c2.Y - c1.Y), Panel.Plane.XY, Panel.Facing.Out, new Vector2(0,0), new Vector2(1,1), mod);
            var back = new Panel(c2, new Vector2(c1.X - c2.X, c1.Y - c2.Y), Panel.Plane.XY, Panel.Facing.In, new Vector2(0,0), new Vector2(1,1), mod);
            var right = new Panel(new Vector3(c2.X, c1.Y, c1.Z), new Vector2(c2.Z - c1.Z, c2.Y - c1.Y), Panel.Plane.YZ,
                Panel.Facing.Out, new Vector2(0,0), new Vector2(1,1), mod);
            var left = new Panel(new Vector3(c1.X, c2.Y, c2.Z), new Vector2(c1.Z - c2.Z, c1.Y - c2.Y), Panel.Plane.YZ,
                Panel.Facing.In, new Vector2(0,0), new Vector2(1,1), mod);
            var top = new Panel(new Vector3(c1.X, c2.Y, c1.Z), new Vector2(c2.X - c1.X, c2.Z - c1.Z), Panel.Plane.XZ,
                Panel.Facing.Out, new Vector2(0,0), new Vector2(1,1), mod);
            var bottom = new Panel(new Vector3(c2.X, c1.Y, c2.Z), new Vector2(c1.X - c2.X, c1.Z - c2.Z), Panel.Plane.XZ,
                Panel.Facing.In, new Vector2(0,0), new Vector2(1,1), mod);

            m_Aggregate = new AggregateGeometry(front, back, right, left, top, bottom);
        }

        public IEnumerable<ushort> Indices
        {
            get { return m_Aggregate.Indices; }
        }

        public IEnumerable<VertexPosNormalTextureMod> Vertices
        {
            get { return m_Aggregate.Vertices; }
        }
    }
}