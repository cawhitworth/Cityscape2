using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

namespace CityScape2.Buildings
{
    class BuildingTexture : Component
    {
        private readonly int m_WindowWidth;
        private readonly int m_WindowHeight;
        private readonly int m_Height;
        private readonly int m_Width;
        private readonly Random m_Rand = new Random();
        private readonly Texture2D m_Texture;

        public BuildingTexture(Device device, DeviceContext context, Size2 textureSize, Size2 windowSize)
        {
            m_Height = textureSize.Height;
            m_Width = textureSize.Width;
            m_WindowWidth = windowSize.Width;
            m_WindowHeight = windowSize.Height;

            var p = Pixels();

            var stream = new DataStream(m_Width*m_Height*4, true, true);
            stream.WriteRange(p);
            var data = new DataBox(stream.DataPointer, m_Width * 4, m_Width * m_Height * 4);

            m_Texture = new Texture2D(device, new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R8G8B8A8_UNorm,
                Height = m_Height,
                Width = m_Width,
                MipLevels = 0,
                OptionFlags = ResourceOptionFlags.GenerateMipMaps,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            });

            context.UpdateSubresource(data, m_Texture);
            var textureView = ToDispose(new ShaderResourceView(device, m_Texture));
            context.GenerateMips(textureView);
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

            // Random windows

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

            // Clusters

            for (int streak = 0; streak < 10; streak++)
            {
                int startX = m_Rand.Next(m_Width / m_WindowWidth);
                int startY = m_Rand.Next(m_Height / m_WindowHeight);
                int width = m_Rand.Next(m_Width / (4 * m_WindowWidth)) + 1;
                if (width + startX > (m_Width / m_WindowWidth)) width = (m_Width / m_WindowWidth) - startX;
                int lines = m_Rand.Next(2) + 1;
                if (startY - lines < 0) startY = lines;
                for (int line = 0; line < lines; line++)
                {
                    for (int xx = startX; xx < startX + width; xx++)
                    {
                        float shade = 0.65f + (float)m_Rand.NextDouble() * 0.25f;
                        DrawWindow(ref pixels, xx, startY - line, shade);
                    }
                    startX += m_Rand.Next(6) - 3;
                    if (startX < 0) startX = 0;
                    if (width + startX > (m_Width / m_WindowWidth)) width = (m_Width / m_WindowWidth) - startX;
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