using System.Collections.Generic;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;

namespace CityScape2
{
    class City : Component
    {
        private readonly DeviceContext m_Context;
        private readonly BatchedGeometryRenderer m_BatchedRenderer;
        private readonly VertexPosNormalTextureShader m_VertexShader;
        private readonly PixelTextureLightShader m_PixelShader;


        public City(Device device, DeviceContext context)
        {
            m_Context = context;

            var buildingTexture = new BuildingTexture(device);
            var texture = Texture.FromTexture2D(buildingTexture.Texture, device);
            m_PixelShader = new PixelTextureLightShader(device, texture);
            m_VertexShader = new VertexPosNormalTextureShader(device);

            var boxes = new List<IGeometry>();

            for (int x = -2; x < 3; x++)
            {
                for (int y = -2; y < 3; y++)
                {
                    boxes.Add(new Box(new Vector3(x - 0.3f, -0.5f, y - 0.3f), new Vector3(x + 0.3f, 0.5f, y + 0.3f)));
                    boxes.Add(new Box(new Vector3(x - 0.4f, -0.6f, y - 0.4f), new Vector3(x + 0.4f, -0.5f, y + 0.4f)));

                }
            }

            var geometryBatcher = new GeometryBatcher(boxes, 3000);

            var vertexSize = Utilities.SizeOf<Vector3>()*2 + Utilities.SizeOf<Vector2>();

            m_BatchedRenderer = new BatchedGeometryRenderer(geometryBatcher, device, vertexSize, m_VertexShader.Layout);

        }


        public int Draw(long elapsed, Matrix view, Matrix proj)
        {
            float fElapsed = elapsed / 1000.0f;
            var world = Matrix.RotationY(fElapsed);// * Matrix.RotationX(fElapsed * 0.7f);
            world.Transpose();

            m_VertexShader.Bind(m_Context, world, view, proj);
            m_PixelShader.Bind(m_Context);

            return m_BatchedRenderer.Render(m_Context);

        }
    }
}