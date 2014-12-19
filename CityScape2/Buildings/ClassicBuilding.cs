using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using CityScape2.Geometry;
using CityScape2.Rendering;
using SharpDX;

namespace CityScape2.Buildings
{
    class ClassicBuilding : IGeometry
    {
        private AggregateGeometry m_Geometry;
        private Random m_Random;

        public ClassicBuilding(Vector3 corner, int widthStories, int heightStories, int depthStories, StoryCalculator storyCalc)
        {
            var builder = new ColumnedBuildingBlockBuilder(storyCalc);

            m_Random = new Random();
            int totalHeight = 0;

            var geometry = new List<IGeometry>();

            var oppCorner = new Vector3(corner.X + widthStories*storyCalc.StorySize, 
                                        corner.Y - 0.5f,
                                        corner.Z + depthStories*storyCalc.StorySize);

            var center = (oppCorner + corner)/2;
            center.Y = 0.0f;

            // Base of the building

            geometry.Add(new Box(corner, oppCorner));

            var tierScale = 0.6 + (m_Random.NextDouble() * 0.4);

            widthStories -= 1;
            depthStories -= 1;
            while (totalHeight < heightStories)
            {
                corner.X = center.X - (widthStories/2.0f) * storyCalc.StorySize;
                corner.Z = center.Z - (depthStories/2.0f) * storyCalc.StorySize;
                corner.Y = center.Y + (totalHeight*storyCalc.StorySize);

                var tierHeight = 0;

                if (heightStories - totalHeight < 5)
                {
                    tierHeight = heightStories - totalHeight;
                }
                else
                {
                    tierHeight = heightStories*2;

                    while (totalHeight + tierHeight > heightStories && tierHeight != 0)
                    {
                        if (heightStories - totalHeight > totalHeight/3)
                        {
                            tierHeight = m_Random.Next((heightStories*5)/6) + (heightStories/6);
                        }
                        else
                        {
                            tierHeight = heightStories - totalHeight;
                        }
                    }
                }

                Console.WriteLine("Corner {0} size {1},{2} height {3} tierHeight {4} totalheight {5}", corner, widthStories, depthStories, totalHeight, tierHeight, heightStories);

                geometry.Add(builder.Build(corner, widthStories, tierHeight, depthStories));

                totalHeight += tierHeight;

                widthStories =(int) (tierScale*widthStories);
                depthStories =(int) (tierScale*depthStories);
                if (widthStories < 1) widthStories = 1;
                if (depthStories < 1) depthStories = 1;

            }


            m_Geometry = new AggregateGeometry(geometry);
        }

        public IEnumerable<ushort> Indices
        {
            get { return m_Geometry.Indices; }
        }

        public IEnumerable<VertexPosNormalTextureMod> Vertices
        {
            get { return m_Geometry.Vertices; }
        }
    }
}
