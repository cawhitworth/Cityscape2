using SharpDX;
using SharpDX.Direct3D11;

namespace CityScape2
{
    class Texture : Component
    {
        private Texture2D m_Texture;
        private readonly ShaderResourceView m_TextureView;
        private readonly SamplerState m_Sampler;

        private Texture(Texture2D t2d, Device device)
        {
            m_Texture = ToDispose(t2d);
            m_TextureView = ToDispose(new ShaderResourceView(device, m_Texture));
            m_Sampler = ToDispose(new SamplerState(device, new SamplerStateDescription
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                BorderColor = Color.Black,
                ComparisonFunction = Comparison.Never,
                MaximumAnisotropy = 16,
                MipLodBias = 0,
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            }));
        }

        public static Texture FromFile(string filename, Device device)
        {
            return new Texture(Texture2D.FromFile<Texture2D>(device, "texture.png"), device);
        }

        public static Texture FromTexture2D(Texture2D t2d, Device device)
        {
            return new Texture(t2d, device);
        }

        public void Bind(DeviceContext context, int slot)
        {
            context.PixelShader.SetSampler(0, m_Sampler);
            context.PixelShader.SetShaderResource(0, m_TextureView);
        }
    }
}