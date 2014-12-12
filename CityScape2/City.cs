using System;
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

            var windowSize = new Size2(8,8);
            var textureSize = new Size2(256,256);

            var storyCalculator = new StoryCalculator(textureSize, windowSize, 0.128f);
            var buildingBuilder = new BuildingBlockBuilder(storyCalculator);
            var buildingTexture = new BuildingTexture(device, context, textureSize, windowSize);

            var texture = Texture.FromTexture2D(buildingTexture.Texture, device);

            m_PixelShader = new PixelTextureLightShader(device, texture);
            m_VertexShader = new VertexPosNormalTextureShader(device);

            var boxes = new List<IGeometry>();
            var r = new Random();

            for (int x = -20; x < 21; x++)
            {
                for (int y = -20; y < 21; y++)
                {
                    boxes.Add(buildingBuilder.Build(
                        new Vector3(x - 0.5f, 0, y - 0.5f),
                        8, r.Next(32), 8));

                }
            }

            var geometryBatcher = new GeometryBatcher(boxes, 3000);

            var vertexSize = Utilities.SizeOf<Vector3>()*2 + Utilities.SizeOf<Vector2>();

            m_BatchedRenderer = new BatchedGeometryRenderer(geometryBatcher, device, vertexSize, m_VertexShader.Layout);

        }


        public int Draw(long elapsed, Matrix view, Matrix proj)
        {
            float fElapsed = elapsed / 1000.0f;
            var world = Matrix.RotationY(fElapsed * 0.2f);// * Matrix.RotationX(fElapsed * 0.7f);
            world.Transpose();

            m_VertexShader.Bind(m_Context, world, view, proj);
            m_PixelShader.Bind(m_Context);

            return m_BatchedRenderer.Render(m_Context);

        }
    }
}