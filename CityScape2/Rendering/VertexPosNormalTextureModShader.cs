using System.IO;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace CityScape2.Rendering
{
    class VertexPosNormalTextureModShader : Component
    {
        private readonly Buffer m_ConstantBuffer;
        private readonly VertexShader m_VertexShader;
        private readonly InputLayout m_Layout;

        public VertexPosNormalTextureModShader(Device device)
        {
            var vertShaderBytecode = File.ReadAllBytes("VertexShader.cso");
            m_VertexShader = ToDispose(new VertexShader(device, vertShaderBytecode));

            m_Layout = ToDispose(new InputLayout(device, vertShaderBytecode, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0,0),
                new InputElement("NORMAL", 0, Format.R32G32B32_Float, 12, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 24, 0), 
                new InputElement("TEXCOORD", 1, Format.R32G32B32_Float, 32, 0) 
            }));

            m_ConstantBuffer =
                ToDispose(new Buffer(device, Utilities.SizeOf<Matrix>() * 3, ResourceUsage.Dynamic,
                    BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));
        }

        public InputLayout Layout { get { return m_Layout; } }

        public void Bind(DeviceContext context, Matrix world, Matrix view, Matrix proj)
        {
            context.VertexShader.Set(m_VertexShader);
            context.VertexShader.SetConstantBuffer(0, m_ConstantBuffer);

            DataStream mappedResource;
            context.MapSubresource(m_ConstantBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
            mappedResource.Write(world);
            mappedResource.Write(view);
            mappedResource.Write(proj);
            context.UnmapSubresource(m_ConstantBuffer, 0);
        }
    }
}