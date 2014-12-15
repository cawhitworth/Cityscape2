using System;

namespace CityScape2
{
    class Overlay
    {
        private long m_Last;
        private int m_Frames;
        private long m_Elapsed;
        private float m_FPS;

        public Overlay(long initial)
        {
            m_Last = initial;
        }

        public void Draw(long elapsed, int polygons)
        {
            m_Frames++;
            m_Elapsed += elapsed - m_Last;
            m_Last = elapsed;

            if (m_Elapsed > 1000)
            {
                m_FPS = m_Frames/(m_Elapsed/1000.0f);
                m_Frames = 0;
                m_Elapsed = 0;
                Console.WriteLine("{0} fps, {1} polys/frame ({2} kpolys/sec)", m_FPS, polygons, (polygons * m_FPS) / 1000);
            }

        }
    }
}