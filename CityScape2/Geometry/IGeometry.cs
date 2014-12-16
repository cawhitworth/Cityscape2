using System.Collections.Generic;
using CityScape2.Rendering;

namespace CityScape2.Geometry
{
    internal interface IGeometry
    {
        IEnumerable<ushort> Indices { get; }
        IEnumerable<VertexPosNormalTextureMod> Vertices { get; }
    }
}