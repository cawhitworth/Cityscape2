using System.Collections.Generic;

namespace CityScape2
{
    internal interface IGeometry
    {
        IEnumerable<ushort> Indices { get; }
        IEnumerable<VertexPosNormalTextureMod> Vertices { get; }
    }
}