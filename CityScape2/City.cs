using System;
using System.Collections.Generic;
using CityScape2.Buildings;
using CityScape2.Geometry;
using CityScape2.Rendering;
using SharpDX;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;

namespace CityScape2
{
    class City : Component
    {
        private readonly DeviceContext m_Context;
        private readonly BatchedGeometryRenderer m_BatchedRenderer;
        private readonly VertexPosNormalTextureModShader m_VertexShader;
        private readonly PixelTextureLightShader m_PixelShader;


        public City(Device device, DeviceContext context)
        {
            m_Context = context;

            var windowSize = new Size2(8,8);
            var textureSize = new Size2(512,512);

            var storyCalculator = new StoryCalculator(textureSize, windowSize, 0.05f);
            var buildingBuilder = new BuildingBlockBuilder(storyCalculator);
            var buildingTexture = new BuildingTexture(device, context, textureSize, windowSize);

            var texture = Texture.FromTexture2D(buildingTexture.Texture, device);

            m_PixelShader = new PixelTextureLightShader(device, texture);
            m_VertexShader = new VertexPosNormalTextureModShader(device);

            var boxes = new List<IGeometry>();
            var r = new Random();
            var panelledBuilder = new PanelledBuildingBlockBuilder(storyCalculator);
            for (int x = -40; x < 41; x++)
            {
                for (int y = -40; y < 41; y++)
                {
                    int height, xSize, zSize;
                    var group = r.Next(10);
                    if (group < 3)
                    {
                        height = r.Next(10);
                        xSize = r.Next(10) + 10;
                        zSize = r.Next(10) + 10;
                    }
                    else if (group < 9)
                    {
                        height = r.Next(30);
                        xSize = r.Next(18) + 2;
                        zSize = r.Next(18) + 2;
                    }
                    else
                    {
                        height = r.Next(60);
                        xSize = r.Next(15) + 5;
                        zSize = r.Next(15) + 5;
                    }

                    height += 1;

                    var c1 = new Vector3(x - 0.5f, 0, y - 0.5f);


                    c1.X += ((20 - xSize)*storyCalculator.StorySize)/2.0f;
                    c1.Z += ((20 - zSize)*storyCalculator.StorySize)/2.0f;

                    if (r.Next(10) > 7)
                    {
                        boxes.Add(buildingBuilder.Build(
                            c1,
                            xSize, height, zSize));
                    }
                    else
                    {
                        boxes.Add(panelledBuilder.Build(c1, xSize, height, zSize));
                    }
                    boxes.Add(new Box(new Vector3(x - 0.5f, -0.5f, y - 0.5f), new Vector3(x + 0.5f, 0.0f, y + 0.5f) ));
                }
            }
            var geometryBatcher = new GeometryBatcher(boxes, 3000);

            var vertexSize = Utilities.SizeOf<Vector3>()*3 + Utilities.SizeOf<Vector2>();

            m_BatchedRenderer = new BatchedGeometryRenderer(geometryBatcher, device, vertexSize, m_VertexShader.Layout);

        }


        public int Draw(long elapsed, Matrix view, Matrix proj)
        {
            //float fElapsed = elapsed / 1000.0f;
            //var world = Matrix.RotationY(fElapsed * 0.2f);// * Matrix.RotationX(fElapsed * 0.7f);

            var world = Matrix.Identity;
            world.Transpose();

            m_VertexShader.Bind(m_Context, world, view, proj);
            m_PixelShader.Bind(m_Context);

            return m_BatchedRenderer.Render(m_Context);

        }
    }
}