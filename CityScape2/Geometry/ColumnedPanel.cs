using System;
using System.Collections.Generic;
using CityScape2.Buildings;
using CityScape2.Rendering;
using SharpDX;

namespace CityScape2.Geometry
{
    class ColumnedPanel : IGeometry
    {
        private readonly IGeometry m_Aggregate;

        public ColumnedPanel(Vector3 position, int storiesHigh, IEnumerable<int> storyWidths, Panel.Plane plane, Panel.Facing facing, Color mod, StoryCalculator storyCalc)
        {
            var panels = new List<IGeometry>();

            bool textured = true;

            var origin = new Vector2(0.0f);
            foreach (var width in storyWidths)
            {
                var tx1 = storyCalc.RandomPosition();
                var tx2 = new Vector2(tx1.X + width, tx1.Y + storiesHigh);
                if (!textured)
                {
                    tx1 = origin;
                    tx2 = origin;
                }
                var direction = facing == Panel.Facing.Out ? 1.0f : -1.0f;

                var size = new Vector2(width*storyCalc.StorySize * direction, storiesHigh*storyCalc.StorySize * direction);
                panels.Add(new Panel(position, size, plane, facing, storyCalc.ToTexture(tx1), storyCalc.ToTexture(tx2), mod));

                switch (plane)
                {
                    case Panel.Plane.XY:
                    case Panel.Plane.XZ:
                        position.X += size.X;
                        break;
                    case Panel.Plane.YZ:
                        position.Z += size.X;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("plane");
                }

                textured = !textured;
            }

            m_Aggregate = new AggregateGeometry(panels);
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