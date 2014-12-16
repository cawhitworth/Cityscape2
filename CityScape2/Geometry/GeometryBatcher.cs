using System.Collections.Generic;
using System.Linq;
using CityScape2.Rendering;

namespace CityScape2.Geometry
{
    class GeometryBatcher : IGeometryBatcher
    {
        private readonly List<ushort[]> m_IndexBatches = new List<ushort[]>();
        private readonly List<VertexPosNormalTextureMod[]> m_VertexBatches = new List<VertexPosNormalTextureMod[]>();
        private readonly int m_MaxIndexBatchSize;
        private readonly int m_MaxVertexBatchSize;

        public GeometryBatcher(IEnumerable<IGeometry> geometries, int desiredBatchSize)
        {
            var currentVertexBatch = new List<VertexPosNormalTextureMod>();
            var currentIndexBatch = new List<ushort>();

            m_MaxIndexBatchSize = 0;
            var indexBase = 0;
            foreach (var geometry in geometries)
            {
                if (currentIndexBatch.Count + geometry.Indices.Count() > desiredBatchSize*3)
                {
                    if (currentIndexBatch.Count > m_MaxIndexBatchSize)
                        m_MaxIndexBatchSize = currentIndexBatch.Count;
                    if (currentVertexBatch.Count > m_MaxVertexBatchSize)
                        m_MaxVertexBatchSize = currentVertexBatch.Count;

                    m_IndexBatches.Add(currentIndexBatch.ToArray());
                    m_VertexBatches.Add(currentVertexBatch.ToArray());
                    currentIndexBatch.Clear();
                    currentVertexBatch.Clear();
                    indexBase = 0;
                }

                currentVertexBatch.AddRange(geometry.Vertices);
                currentIndexBatch.AddRange(geometry.Indices.Select(i => (ushort)(i + indexBase)));
                indexBase += geometry.Vertices.Count();
            }

            if (currentIndexBatch.Count > m_MaxIndexBatchSize)
                m_MaxIndexBatchSize = currentIndexBatch.Count;
            if (currentVertexBatch.Count > m_MaxVertexBatchSize)
                m_MaxVertexBatchSize = currentVertexBatch.Count;
            m_IndexBatches.Add(currentIndexBatch.ToArray());
            m_VertexBatches.Add(currentVertexBatch.ToArray());
        }

        public int MaxVertexBatchSize
        {
            get { return m_MaxVertexBatchSize; }
        }

        public int MaxIndexBatchSize
        {
            get { return m_MaxIndexBatchSize; }
        }

        public IEnumerable<ushort[]> IndexBatches
        {
            get { return m_IndexBatches; }
        }

        public IEnumerable<VertexPosNormalTextureMod[]> VertexBatches
        {
            get { return m_VertexBatches; }
        }
    }
}