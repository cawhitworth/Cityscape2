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
        private readonly Random m_Random;
        private readonly StoryCalculator m_StoryCalculator;
        private readonly BuildingBlockBuilder m_BuildingBuilder;
        private readonly ColumnedBuildingBlockBuilder m_ColumnedBuilder;


        public City(Device device, DeviceContext context)
        {
            m_Context = context;

            var windowSize = new Size2(8,8);
            var textureSize = new Size2(512,512);

            m_StoryCalculator = new StoryCalculator(textureSize, windowSize, 0.05f);
            m_BuildingBuilder = new BuildingBlockBuilder(m_StoryCalculator);
            m_ColumnedBuilder = new ColumnedBuildingBlockBuilder(m_StoryCalculator);

            var buildingTexture = new BuildingTexture(device, context, textureSize, windowSize);

            var texture = Texture.FromTexture2D(buildingTexture.Texture, device);

            m_PixelShader = new PixelTextureLightShader(device, texture);
            m_VertexShader = new VertexPosNormalTextureModShader(device);

            var boxes = new List<IGeometry>();
            m_Random = new Random();

            for (int x = -40; x < 41; x++)
            {
                for (int y = -40; y < 41; y++)
                {
                    boxes.Add(MakeBuilding(x, y));
                }
            }
            var geometryBatcher = new GeometryBatcher(boxes, 3000);

            var vertexSize = Utilities.SizeOf<Vector3>()*3 + Utilities.SizeOf<Vector2>();

            m_BatchedRenderer = new BatchedGeometryRenderer(geometryBatcher, device, vertexSize, m_VertexShader.Layout);

        }

        private IGeometry MakeBuilding(int x, int y)
        {
            int yStories, xStories, zStories;
            PickBuildingSize(out xStories, out yStories, out zStories);

            var c1 = new Vector3(x - 0.5f, 0, y - 0.5f);

            c1.X += ((20 - xStories)*m_StoryCalculator.StorySize)/2.0f;
            c1.Z += ((20 - zStories)*m_StoryCalculator.StorySize)/2.0f;

            IGeometry building;

            var buildingPick = m_Random.Next(10);

            if (yStories > 15)
            {
                if (buildingPick > 7)
                {
                    building = m_BuildingBuilder.Build(c1, xStories, yStories, zStories);
                }
                else
                {
                    building = new ClassicBuilding(c1, xStories, yStories, zStories, m_StoryCalculator);
                }
            }
            else
            {
                if (buildingPick > 7)
                {
                    building = m_BuildingBuilder.Build(c1, xStories, yStories, zStories);
                }
                else 
                {
                    building = m_ColumnedBuilder.Build(c1, xStories, yStories, zStories);
                }
            }

            var buildingBase = new Box(new Vector3(x - 0.5f, -0.5f, y - 0.5f), new Vector3(x + 0.5f, 0.0f, y + 0.5f));

            return new AggregateGeometry(buildingBase, building);
        }

        private void PickBuildingSize(out int xSize, out int ySize, out int zSize)
        {
            var sizeGroup = m_Random.Next(10);
            if (sizeGroup < 3)
            {
                ySize = m_Random.Next(10);
                xSize = m_Random.Next(10) + 10;
                zSize = m_Random.Next(10) + 10;
            }
            else if (sizeGroup < 9)
            {
                ySize = m_Random.Next(30);
                xSize = m_Random.Next(18) + 2;
                zSize = m_Random.Next(18) + 2;
            }
            else
            {
                ySize = m_Random.Next(60);
                xSize = m_Random.Next(15) + 5;
                zSize = m_Random.Next(15) + 5;
            }

            ySize += 1;
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