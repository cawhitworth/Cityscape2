using System;
using System.Collections;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using CityScape2.Geometry;
using SharpDX;

namespace CityScape2.Buildings
{
    class BuildingBlockBuilder
    {
        private readonly StoryCalculator m_StoryCalc;
        private readonly ModColor m_ModColor;

        public BuildingBlockBuilder(StoryCalculator storyCalc)
        {
            m_StoryCalc = storyCalc;
            m_ModColor = new ModColor(new Random());
        }

        public IGeometry Build(Vector3 c1, int xStories, int yStories, int zStories)
        {
            var c2 = new Vector3(c1.X + xStories * m_StoryCalc.StorySize, c1.Y + yStories * m_StoryCalc.StorySize, c1.Z + zStories * m_StoryCalc.StorySize);

            var mod = m_ModColor.Pick();

            var tx1 = m_StoryCalc.RandomPosition();
            var tx2 = new Vector2(tx1.X + xStories, tx1.Y + yStories);

            var front = new Panel(c1, new Vector2(c2.X - c1.X, c2.Y - c1.Y), Panel.Plane.XY, Panel.Facing.Out, m_StoryCalc.ToTexture(tx1), m_StoryCalc.ToTexture(tx2), mod);

            tx1 = m_StoryCalc.RandomPosition();
            tx2 = new Vector2(tx1.X + xStories, tx1.Y - yStories);// The corner coordinates get flipped for back/left

            var back = new Panel(c2, new Vector2(c1.X - c2.X, c1.Y - c2.Y), Panel.Plane.XY, Panel.Facing.In, m_StoryCalc.ToTexture(tx1), m_StoryCalc.ToTexture(tx2), mod);

            tx1 = m_StoryCalc.RandomPosition();
            tx2 = new Vector2(tx1.X + zStories, tx1.Y + yStories); 

            var right = new Panel(new Vector3(c2.X, c1.Y, c1.Z), new Vector2(c2.Z - c1.Z, c2.Y - c1.Y), Panel.Plane.YZ,
                Panel.Facing.Out, m_StoryCalc.ToTexture(tx1), m_StoryCalc.ToTexture(tx2), mod);

            tx1 = m_StoryCalc.RandomPosition();
            tx2 = new Vector2(tx1.X + zStories, tx1.Y - yStories);

            var left = new Panel(new Vector3(c1.X, c2.Y, c2.Z), new Vector2(c1.Z - c2.Z, c1.Y - c2.Y), Panel.Plane.YZ,
                Panel.Facing.In, m_StoryCalc.ToTexture(tx1), m_StoryCalc.ToTexture(tx2), mod);

            tx1 = new Vector2(0,0);

            var top = new Panel(new Vector3(c1.X, c2.Y, c1.Z), new Vector2(c2.X - c1.X, c2.Z - c1.Z), Panel.Plane.XZ,
                Panel.Facing.Out, tx1, tx1, mod);

            var aggregate = new AggregateGeometry(front, back, right, left, top );

            return aggregate;
        }
    }
}