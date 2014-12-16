using System;
using SharpDX;

namespace CityScape2.Buildings
{
    class StoryCalculator
    {
        private readonly Size2 m_TextureSize;
        private readonly Size2 m_WindowSize;
        private readonly float m_StorySize;
        private readonly Random m_Random;

        public StoryCalculator(Size2 textureSize, Size2 windowSize, float storySize)
        {
            m_TextureSize = textureSize;
            m_WindowSize = windowSize;
            m_StorySize = storySize;
            m_Random = new Random();
        }

        public float StorySize
        {
            get { return m_StorySize; }
        }

        public float ToTextureX(int stories)
        {
            return stories*((float)m_WindowSize.Width/m_TextureSize.Width);
        }

        public float ToTextureY(int stories)
        {
            return stories*((float)m_WindowSize.Height/m_TextureSize.Height);
        }

        public int StoriesX { get { return m_TextureSize.Width/m_WindowSize.Width; } }
        public int StoriesY { get { return m_TextureSize.Height/m_WindowSize.Height; } }

        public Vector2 RandomPosition()
        {
            return new Vector2(m_Random.Next(StoriesX), m_Random.Next(StoriesY));
        }

        public Vector2 ToTexture(Vector2 p)
        {
            return new Vector2(ToTextureX((int)p.X), ToTextureY((int)p.Y));
        }
    }
}