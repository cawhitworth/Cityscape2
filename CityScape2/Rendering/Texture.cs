using SharpDX;
using SharpDX.Direct3D11;

namespace CityScape2.Rendering
{
    class Texture : Component
    {
        private readonly ShaderResourceView m_TextureView;
        private readonly SamplerState m_Sampler;

        private Texture(Texture2D t2D, Device device)
        {
            Texture2D texture = ToDispose(t2D);
            m_TextureView = ToDispose(new ShaderResourceView(device, texture));
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
            return new Texture(Resource.FromFile<Texture2D>(device, "texture.png"), device);
        }

        public static Texture FromTexture2D(Texture2D t2D, Device device)
        {
            return new Texture(t2D, device);
        }

        public void Bind(DeviceContext context, int slot)
        {
            context.PixelShader.SetSampler(slot, m_Sampler);
            context.PixelShader.SetShaderResource(slot, m_TextureView);
        }
    }
}