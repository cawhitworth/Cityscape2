using System.Linq;
using CityScape2.Geometry;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace CityScape2.Rendering
{
    class BatchedGeometryRenderer : Component
    {
        private readonly Buffer m_Vertices;
        private readonly Buffer m_Indices;
        private readonly IGeometryBatcher m_GeometryBatcher;
        private readonly int m_VertexSize;
        private readonly InputLayout m_Layout;

        public BatchedGeometryRenderer(IGeometryBatcher batcher, Device device, int vertexSize, InputLayout layout)
        {
            m_Vertices =
                ToDispose(new Buffer(device, vertexSize*batcher.MaxVertexBatchSize, ResourceUsage.Dynamic, BindFlags.VertexBuffer,
                    CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
            m_Indices = 
                ToDispose(new Buffer(device, batcher.MaxIndexBatchSize * 2, ResourceUsage.Dynamic, BindFlags.IndexBuffer, 
                    CpuAccessFlags.Write, ResourceOptionFlags.None, 0));

            m_GeometryBatcher = batcher;
            m_VertexSize = vertexSize;
            m_Layout = layout;
        }

        public int Render(DeviceContext context)
        {
            context.InputAssembler.InputLayout = m_Layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(m_Vertices, m_VertexSize, 0));
            context.InputAssembler.SetIndexBuffer(m_Indices, Format.R16_UInt, 0);

            return m_GeometryBatcher.VertexBatches.Zip(m_GeometryBatcher.IndexBatches, (vertices, indices) =>
            {
                DataStream mappedResource;
                context.MapSubresource(m_Indices, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
                mappedResource.WriteRange(indices);
                context.UnmapSubresource(m_Indices, 0);

                context.MapSubresource(m_Vertices, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
                mappedResource.WriteRange(vertices);
                context.UnmapSubresource(m_Vertices, 0);

                context.DrawIndexed(indices.Count(), 0, 0);

                return indices.Count();
            }).Aggregate(0, (a, b) => a + b) / 3;
        }
    }
}