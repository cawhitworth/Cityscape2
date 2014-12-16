using System.Collections.Generic;
using System.Linq;
using CityScape2.Rendering;

namespace CityScape2.Geometry
{
    class AggregateGeometry : IGeometry
    {
        private ushort[] m_Indices;
        private VertexPosNormalTextureMod[] m_Vertices;

        public AggregateGeometry(params IGeometry[] geometries)
        {
            Aggregate(geometries);
        }

        public AggregateGeometry(IEnumerable<IGeometry> geometries)
        {
            Aggregate(geometries);
        }

        private void Aggregate(IEnumerable<IGeometry> geometries)
        {
            var allIndices = new List<ushort>();
            var allVertices = new List<VertexPosNormalTextureMod>();
            ushort baseIndex = 0;

            foreach (var geometry in geometries)
            {
                allIndices.AddRange(geometry.Indices.Select(i => (ushort) (i + baseIndex)));
                allVertices.AddRange(geometry.Vertices);
                baseIndex += (ushort)geometry.Vertices.Count();
            }
            m_Indices = allIndices.ToArray();
            m_Vertices = allVertices.ToArray();
        }

        public IEnumerable<ushort> Indices { get { return m_Indices; }}
        public IEnumerable<VertexPosNormalTextureMod> Vertices { get { return m_Vertices; }}
    }
}