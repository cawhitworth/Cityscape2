﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace CityScape2
{
    class City : Component
    {
        private readonly Device m_Device;
        private readonly DeviceContext m_Context;
        private VertexShader m_VertexShader;
        private InputLayout m_Layout;
        private PixelShader m_PixelShader;
        private Buffer m_ConstantBuffer;
        private Texture2D m_Texture;
        private ShaderResourceView m_TextureView;
        private SamplerState m_Sampler;
        private Buffer m_Vertices;
        private Buffer m_Indices;
        private int m_IndexCount;

        public City(Device device, DeviceContext context)
        {
            m_Device = device;
            m_Context = context;

            LoadShaders();
            LoadTextures();

            var boxes = new List<IGeometry>();

            for(int x = -20; x < 20; x++)
                for (int y = -20; y < 20; y++)
                    boxes.Add(new Box(new Vector3(x - 0.4f, -0.4f, y - 0.4f), new Vector3(x + 0.4f, 0.4f, y + 0.4f)));

            var aggregate = new AggregateGeometry(boxes);

            m_Vertices = ToDispose(Buffer.Create(m_Device, BindFlags.VertexBuffer, aggregate.Vertices.ToArray()));
            m_Indices = ToDispose(Buffer.Create(m_Device, BindFlags.IndexBuffer, aggregate.Indices.ToArray()));
            m_IndexCount = aggregate.Indices.Count();
        }

        private void LoadTextures()
        {
            m_Texture = ToDispose(Texture2D.FromFile<Texture2D>(m_Device, "texture.png"));
            m_TextureView = ToDispose(new ShaderResourceView(m_Device, m_Texture));
            m_Sampler = new SamplerState(m_Device, new SamplerStateDescription
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
                MaximumLod = 16
            });
        }

        private void LoadShaders()
        {
            var vertShaderBytecode = File.ReadAllBytes("VertexShader.cso");
            m_VertexShader = ToDispose(new VertexShader(m_Device, vertShaderBytecode));

            m_Layout = ToDispose(new InputLayout(m_Device, vertShaderBytecode, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0,0),
                new InputElement("NORMAL", 0, Format.R32G32B32_Float, 12, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 24, 0), 
            }));

            var pixelShaderBytecode = File.ReadAllBytes("PixelShader.cso");
            m_PixelShader = ToDispose(new PixelShader(m_Device, pixelShaderBytecode));
            
            m_ConstantBuffer =
                ToDispose(new Buffer(m_Device, Utilities.SizeOf<Matrix>() * 3, ResourceUsage.Dynamic,
                    BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0));

        }

        public int Draw(long elapsed, Matrix view, Matrix proj)
        {
            float fElapsed = elapsed / 1000.0f;
            var world = Matrix.RotationY(fElapsed);// * Matrix.RotationX(fElapsed * 0.7f);
            world.Transpose();

            m_Context.InputAssembler.InputLayout = m_Layout;
            m_Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            m_Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(m_Vertices, Utilities.SizeOf<Vector3>() * 2 + Utilities.SizeOf<Vector2>(), 0));
            m_Context.InputAssembler.SetIndexBuffer(m_Indices, Format.R16_UInt, 0);
            m_Context.VertexShader.SetConstantBuffer(0, m_ConstantBuffer);
            m_Context.VertexShader.Set(m_VertexShader);
            m_Context.PixelShader.Set(m_PixelShader);
            m_Context.PixelShader.SetSampler(0, m_Sampler);
            m_Context.PixelShader.SetShaderResource(0, m_TextureView);

            DataStream mappedResource;
            m_Context.MapSubresource(m_ConstantBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
            mappedResource.Write(world);
            mappedResource.Write(view);
            mappedResource.Write(proj);
            m_Context.UnmapSubresource(m_ConstantBuffer, 0);

            m_Context.DrawIndexed(m_IndexCount, 0, 0);

            return m_IndexCount/3;
        }
    }
}