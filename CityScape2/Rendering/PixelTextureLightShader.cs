using System.IO;
using SharpDX;
using SharpDX.Direct3D11;

namespace CityScape2.Rendering
{
    class PixelTextureLightShader : Component
    {
        private readonly Texture m_Texture;
        private readonly PixelShader m_PixelShader;

        public PixelTextureLightShader(Device device, Texture texture)
        {
            m_Texture = texture;
            var pixelShaderBytecode = File.ReadAllBytes("PixelShader.cso");
            m_PixelShader = ToDispose(new PixelShader(device, pixelShaderBytecode));
        }

        public void Bind(DeviceContext context)
        {
            context.PixelShader.Set(m_PixelShader);
            m_Texture.Bind(context, 0);
        }
    }
}