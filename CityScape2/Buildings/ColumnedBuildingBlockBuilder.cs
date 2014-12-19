using System;
using System.Collections.Generic;
using System.Linq;
using CityScape2.Geometry;
using SharpDX;

namespace CityScape2.Buildings
{
    class ColumnedBuildingBlockBuilder
    {
        private readonly StoryCalculator m_StoryCalc;
        private readonly ModColor m_ModColor;
        private readonly Random m_Random;
        private Func<int, IEnumerable<int>> m_GenerateColumns;

        public ColumnedBuildingBlockBuilder(StoryCalculator storyCalculator, Func<int, IEnumerable<int>> generateColumns = null)
        {
            m_StoryCalc = storyCalculator;
            m_Random = new Random();
            m_ModColor = new ModColor(m_Random);
            if (generateColumns == null)
                generateColumns = GenerateColumns;
            m_GenerateColumns = generateColumns;
        }

        public IGeometry Build(Vector3 c1, int xStories, int yStories, int zStories)
        {
            var c2 = new Vector3(c1.X + xStories*m_StoryCalc.StorySize, c1.Y + yStories*m_StoryCalc.StorySize,
                c1.Z + zStories*m_StoryCalc.StorySize);

            var mod = m_ModColor.Pick();

            var storyWidthsX = m_GenerateColumns(xStories).ToArray();
            var storyWidthsZ = m_GenerateColumns(zStories).ToArray();

            var front = new ColumnedPanel(c1, yStories, storyWidthsX, Panel.Plane.XY, Panel.Facing.Out, mod, m_StoryCalc);
            var back = new ColumnedPanel(c2, yStories, storyWidthsX, Panel.Plane.XY, Panel.Facing.In, mod, m_StoryCalc);

            var right = new ColumnedPanel(new Vector3(c2.X, c1.Y, c1.Z), yStories, storyWidthsZ, Panel.Plane.YZ, Panel.Facing.Out, mod, m_StoryCalc);
            var left = new ColumnedPanel(new Vector3(c1.X, c2.Y, c2.Z), yStories, storyWidthsZ, Panel.Plane.YZ, Panel.Facing.In, mod, m_StoryCalc);

            var tx1 = new Vector2(0,0);

            var top = new Panel(new Vector3(c1.X, c2.Y, c1.Z), new Vector2(c2.X - c1.X, c2.Z - c1.Z), Panel.Plane.XZ,
                Panel.Facing.Out, tx1, tx1, mod);

            var aggregate = new AggregateGeometry(front, back, right, left, top );

            return aggregate;
        }

        IEnumerable<int> GenerateColumns(int width)
        {
            var spacer = new bool[width];
            var offset = 0;
            while (offset < width / 2)
            {
                offset += m_Random.Next((width / 3)) + 2;
                if (offset >= width / 2)
                    break;

                spacer[offset] = true;
                spacer[(width - 1) - offset] = true;

                if (m_Random.Next(2) == 1)
                {
                    spacer[offset+1] = true;
                    spacer[(width - 1) - (offset + 1)] = true;
                    offset += 1;
                }
            }

            var w = 0;
            var window = true;
            foreach (var b in spacer)
            {
                if (window)
                {
                    if (b)
                    {
                        yield return w;
                        window = false;
                        w = 1;
                    }
                    else
                    {
                        w += 1;
                    }
                }
                else
                {
                    if (b)
                    {
                        w += 1;
                    }
                    else
                    {
                        yield return w;
                        window = true;
                        w = 1;
                    }
                }
            }
            yield return w;
        }
    }
}