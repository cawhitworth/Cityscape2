using System.Collections.Generic;
using CityScape2.Rendering;

namespace CityScape2.Geometry
{
    internal interface IGeometryBatcher
    {
        int MaxVertexBatchSize { get; }
        int MaxIndexBatchSize { get; }
        IEnumerable<ushort[]> IndexBatches { get; }
        IEnumerable<VertexPosNormalTextureMod[]> VertexBatches { get; }
    }
}