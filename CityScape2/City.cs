using System.Collections.Generic;
using System.IO;
using SharpDX;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;

namespace CityScape2
{
    class City : Component
    {
        private readonly Device m_Device;
        private readonly DeviceContext m_Context;
        private PixelShader m_PixelShader;
        private readonly Texture m_Texture;
        private readonly BatchedGeometryRenderer m_BatchedRenderer;
        private VertexPosNormalTextureShader m_VertexShader;


        public City(Device device, DeviceContext context)
        {
            m_Device = device;
            m_Context = context;

            LoadShaders();

            m_VertexShader = new VertexPosNormalTextureShader(m_Device);
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

            m_BatchedRenderer = new BatchedGeometryRenderer(geometryBatcher, m_Device, vertexSize, m_VertexShader.Layout);

        }

        private void LoadShaders()
        {

            var pixelShaderBytecode = File.ReadAllBytes("PixelShader.cso");
            m_PixelShader = ToDispose(new PixelShader(m_Device, pixelShaderBytecode));
            

        }

        public int Draw(long elapsed, Matrix view, Matrix proj)
        {
            float fElapsed = elapsed / 1000.0f;
            var world = Matrix.RotationY(fElapsed);// * Matrix.RotationX(fElapsed * 0.7f);
            world.Transpose();

            m_VertexShader.Bind(m_Context, world, view, proj);

            m_Context.PixelShader.Set(m_PixelShader);
            m_Texture.Bind(m_Context, 0);


            return m_BatchedRenderer.Render(m_Context);

        }
    }
}