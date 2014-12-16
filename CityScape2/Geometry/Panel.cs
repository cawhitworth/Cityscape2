using System;
using System.Collections.Generic;
using CityScape2.Rendering;
using SharpDX;

namespace CityScape2.Geometry
{
    public class Panel : IGeometry
    {
        private readonly Vector3 m_Position;
        private readonly Vector2 m_Size;
        private readonly Plane m_Plane;
        private readonly Facing m_Facing;
        private readonly Vector2 m_Tex1;
        private readonly Vector2 m_Tex2;
        private readonly ushort[] m_Indices;
        private readonly VertexPosNormalTextureMod[] m_Vertices;
        private readonly Vector3 m_Mod;

        public enum Plane
        {
            XY,
            YZ,
            XZ
        };

        public enum Facing
        {
            In, Out
        }

        public Panel(Vector3 position, Vector2 size, Plane plane, Facing facing, Vector2 tex1, Vector2 tex2, Color mod)
        {
            m_Position = position;
            m_Size = size;
            m_Plane = plane;
            m_Facing = facing;
            m_Tex1 = tex1;
            m_Tex2 = tex2;
            m_Mod = new Vector3(mod.R / 255.0f, mod.G / 255.0f, mod.B / 255.0f);

            if (m_Facing == Facing.Out)
            {
                m_Indices = new ushort[]
                {
                    0, 1, 2, 1, 3, 2
                };
            }
            else
            {
                m_Indices = new ushort[]
                {
                    0, 2, 1, 3, 1, 2
                };
            }
            m_Vertices = CalculateVertices();
        }

        public IEnumerable<VertexPosNormalTextureMod> Vertices
        {
            get { return m_Vertices; }
        }

        private VertexPosNormalTextureMod[] CalculateVertices()
        {
            Vector3 normal;
            Vector3 offsetVector, oppositeCorner, topLeft, bottomRight;
            switch (m_Plane)
            {
                case Plane.XY:
                    offsetVector = new Vector3(m_Size.X, m_Size.Y, 0.0f);
                    oppositeCorner = m_Position + offsetVector;
                    topLeft = new Vector3(m_Position.X, oppositeCorner.Y, m_Position.Z);
                    bottomRight = new Vector3(oppositeCorner.X, m_Position.Y, m_Position.Z);
                    normal = new Vector3(0, 0, -1);
                    break;
                case Plane.YZ:
                    offsetVector = new Vector3(0.0f, m_Size.Y, m_Size.X);
                    oppositeCorner = m_Position + offsetVector;
                    topLeft = new Vector3(m_Position.X, oppositeCorner.Y, m_Position.Z);
                    bottomRight = new Vector3(m_Position.X, m_Position.Y, oppositeCorner.Z);
                    normal = new Vector3(1, 0, 0);
                    break;
                case Plane.XZ:
                    offsetVector = new Vector3(m_Size.X, 0.0f, m_Size.Y);
                    oppositeCorner = m_Position + offsetVector;
                    topLeft = new Vector3(m_Position.X, m_Position.Y, oppositeCorner.Z);
                    bottomRight = new Vector3(oppositeCorner.X, m_Position.Y, m_Position.Z);
                    normal = new Vector3(0, 1, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (m_Facing == Facing.In)
                normal *= -1;

            var texTopLeft = new Vector2(m_Tex1.X, m_Tex2.Y);
            var texBottomRight = new Vector2(m_Tex2.X, m_Tex1.Y);

            return new[]
            {
                new VertexPosNormalTextureMod(m_Position, normal, m_Tex1, m_Mod),
                new VertexPosNormalTextureMod(topLeft, normal, texTopLeft, m_Mod),
                new VertexPosNormalTextureMod(bottomRight, normal, texBottomRight, m_Mod),
                new VertexPosNormalTextureMod(oppositeCorner, normal, m_Tex2, m_Mod)
            };
        }

        public IEnumerable<ushort> Indices
        {
            get { return m_Indices; }
        }
    }
}