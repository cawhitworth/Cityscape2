using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

namespace CityScape2
{
    class BuildingTexture : Component
    {
        private int m_WindowWidth;
        private int m_WindowHeight;
        private int m_Height;
        private int m_Width;
        private readonly Random m_Rand = new Random();
        private readonly Texture2D m_Texture;

        public BuildingTexture(Device device)
        {
            m_Height = 256;
            m_Width = 256;
            m_WindowWidth = 8;
            m_WindowHeight = 8;

            var p = Pixels();

            var stream = new DataStream(m_Width*m_Height*4, true, true);
            stream.WriteRange(p);
            var data = new DataRectangle(stream.DataPointer, m_Width * 4);

            m_Texture = new Texture2D(device, new Texture2DDescription()
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R8G8B8A8_UNorm,
                Height = m_Height,
                Width = m_Width,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default

            }, new [] {data});
        }

        public Texture2D Texture { get { return m_Texture; }}

        public Color[] Pixels()
        {
            var pixels = new Color[m_Width * m_Height];
            var black = new Color(0);
            for (int p = 0; p < m_Width * m_Height; p++)
            {
                pixels[p] = black;
            }


            for (int x = 0; x < m_Width / m_WindowWidth; x++)
            {
                for (int y = 0; y < m_Height / m_WindowHeight; y++)
                {
                    bool light = m_Rand.NextDouble() > 0.9;
                    float shade = (float)m_Rand.NextDouble() * 0.15f;
                    if (light)
                        shade += 0.75f;

                    DrawWindow(ref pixels, x, y, shade);
                }
            }

            return pixels;
        }

        private void DrawWindow(ref Color[] pixels, int x, int y, float shade)
        {
            var col = new Color(shade, shade, shade, 1.0f);

            for (int xx = 1; xx < m_WindowWidth - 1; xx++)
            {
                for (int yy = 1; yy < m_WindowHeight - 1; yy++)
                {
                    pixels[(x*m_WindowWidth) + xx + ((y*m_WindowHeight) + yy)*m_Width] = col;
                }
            }

            // Add noise

            int points = m_Rand.Next(16) + 16;
            for (int point = 0; point < points; point++)
            {
                int xx = m_Rand.Next(6) + 1;
                int yy = m_Rand.Next(4) + 1;
                Color c = pixels[(x*m_WindowWidth) + xx + ((y*m_WindowHeight) + yy)*m_Width];
                c.R = ModColor(c.R, 2);
                c.G = c.B = c.R;
                c.G = ModColor(c.G, 4);
                c.B = ModColor(c.B, 4);
                pixels[(x*m_WindowWidth) + xx + ((y*m_WindowHeight) + yy)*m_Width] = c;

            }
        }

        byte ModColor(byte component, int divisor)
        {
            var comp = (int)component;
            comp -= m_Rand.Next( component / divisor );
            if (comp > 255) comp = 255;
            if (comp < 0) comp = 0;
            return (byte)comp;
        }
    }
}