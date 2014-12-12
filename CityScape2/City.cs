using System.Collections.Generic;
using System.IO;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace CityScape2
{
    class City : Component
    {
        private readonly Device m_Device;
        private readonly DeviceContext m_Context;
        private VertexShader m_VertexShader;
        private InputLayout m_Layout;
        private PixelShader m_PixelShader;
        private Buffer m_ConstantBuffer;
        private readonly Texture m_Texture;
        private readonly BatchedGeometryRenderer m_BatchedRenderer;

        public City(Device device, DeviceContext context)
        {
            m_Device = device;
            m_Context = context;

            LoadShaders();

            m_Texture = Texture.FromFile("texture.png", m_Device);

            var boxes = new List<IGeometry>();

            for (int x = -50; x < 50; x++)
            {
                for (int y = -50; y < 50; y++)
                {
                    boxes.Add(new Box(new Vector3(x - 0.3f, -0.5f, y - 0.3f), new Vector3(x + 0.3f, 0.5f, y + 0.3f)));
                    boxes.Add(new Box(new Vector3(x - 0.4f, -0.6f, y - 0.4f), new Vector3(x + 0.4f, -0.5f, y + 0.4f)));

                }
            }

            var geometryBatcher = new GeometryBatcher(boxes, 3000);

            var vertexSize = Utilities.SizeOf<Vector3>()*2 + Utilities.SizeOf<Vector2>();

            m_BatchedRenderer = new BatchedGeometryRenderer(geometryBatcher, m_Device, vertexSize, m_Layout);

        }

        private void LoadShaders()
        {
            var vertShaderBytecode = File.ReadAllBytes("VertexShader.cso");
            m_VertexShader = ToDispose(new VertexShader(m_Device, vertShaderBytecode));

            m_Layout = ToDispose(new InputLayout(m_Device, vertShaderBytecode, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0,0),
                new InputElement("NORMAL", 0, Format.R32G32B32_Float, 12, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 24, 0), 
            }));

            var pixelShaderBytecode = File.ReadAllBytes("PixelShader.cso");
            m_PixelShader = ToDispose(new PixelShader(m_Device, pixelShaderBytecode));
            
            m_ConstantBuffer =
                ToDispose(new Buffer(m_Device, Utilities.SizeOf<Matrix>() * 3, ResourceUsage.Dynamic,
                    BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));

        }

        public int Draw(long elapsed, Matrix view, Matrix proj)
        {
            float fElapsed = elapsed / 1000.0f;
            var world = Matrix.RotationY(fElapsed);// * Matrix.RotationX(fElapsed * 0.7f);
            world.Transpose();

            m_Context.VertexShader.Set(m_VertexShader);
            m_Context.VertexShader.SetConstantBuffer(0, m_ConstantBuffer);
            m_Context.PixelShader.Set(m_PixelShader);

            m_Texture.Bind(m_Context, 0);

            DataStream mappedResource;
            m_Context.MapSubresource(m_ConstantBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
            mappedResource.Write(world);
            mappedResource.Write(view);
            mappedResource.Write(proj);
            m_Context.UnmapSubresource(m_ConstantBuffer, 0);

            return m_BatchedRenderer.Render(m_Context);

        }
    }
}