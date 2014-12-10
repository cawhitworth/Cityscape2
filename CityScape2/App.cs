﻿using System;
using System.IO;
using System.Windows.Forms;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace CityScape2
{
    internal class App: Component
    {
        private RenderForm m_Form;
        private Device m_Device;
        private SwapChain m_SwapChain;
        private DeviceContext m_Context;
        private Texture2D m_BackBuffer;
        private RenderTargetView m_RenderView;
        private Texture2D m_DepthBuffer;
        private DepthStencilView m_DepthView;
        private Factory m_Factory;
        private PixelShader m_PixelShader;
        private VertexShader m_VertexShader;
        private InputLayout m_Layout;
        private Matrix m_Proj;

        public void Run()
        {
            CreateDeviceAndSwapChain();

            LoadShaders();

            m_Form.KeyUp += (sender, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                    m_Form.Close();
            };

            var recreate = true;
            m_Form.Resize += (sender, args) => recreate = true;

            RecreateBuffers();

            var vertices = ToDispose(Buffer.Create(m_Device, BindFlags.VertexBuffer, new[]
            {
                new Vector4(-1.0f, -1.0f, 0.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                new Vector4(0.0f, 1.0f, 0.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                new Vector4(1.0f, -1.0f, 0.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            }));

            var view = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY);
            var proj = Matrix.Identity;

            var constantBuffer =
                ToDispose(new Buffer(m_Device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default,
                    BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0));

            RenderLoop.Run(m_Form, () =>
            {
                if (recreate)
                {
                    RecreateBuffers();
                    proj = Matrix.PerspectiveFovLH((float) Math.PI/4.0f, (float) Width/Height, 0.01f, 100.0f);
                    recreate = false;
                }

                m_Context.InputAssembler.InputLayout = m_Layout;
                m_Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                m_Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, Utilities.SizeOf<Vector4>() * 2, 0));
                m_Context.VertexShader.SetConstantBuffer(0, constantBuffer);
                m_Context.VertexShader.Set(m_VertexShader);
                m_Context.PixelShader.Set(m_PixelShader);

                m_Context.ClearDepthStencilView(m_DepthView, DepthStencilClearFlags.Depth, 1.0f, 0);
                m_Context.ClearRenderTargetView(m_RenderView, Color.CornflowerBlue);

                var worldViewProj = Matrix.Multiply(view, proj);
                worldViewProj.Transpose();
                m_Context.UpdateSubresource(ref worldViewProj, constantBuffer);

                m_Context.Draw(3, 0);

                m_SwapChain.Present(0, PresentFlags.None);
            });

            DisposeBuffers();
            Dispose();
        }

        private void LoadShaders()
        {
            var vertShaderBytecode = File.ReadAllBytes("VertexShader.cso");
            m_VertexShader = ToDispose(new VertexShader(m_Device, vertShaderBytecode));

            m_Layout = ToDispose(new InputLayout(m_Device, vertShaderBytecode, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0,0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
            }));

            var pixelShaderBytecode = File.ReadAllBytes("PixelShader.cso");
            m_PixelShader = ToDispose(new PixelShader(m_Device, pixelShaderBytecode));
        }

        private int Width
        {
            get { return m_Form.ClientSize.Width; }
        }

        private int Height
        {
            get { return m_Form.ClientSize.Height; }
        }

        private void CreateDeviceAndSwapChain()
        {
            m_Form = ToDispose(new RenderForm());

            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                IsWindowed = true,
                ModeDescription =
                    new ModeDescription(Width, Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                OutputHandle = m_Form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            // Create device + swapchain
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out m_Device, out m_SwapChain);
            m_Context = ToDispose(m_Device.ImmediateContext);
            m_Device = ToDispose(m_Device);
            m_SwapChain = ToDispose(m_SwapChain);

            // Ignore Windows events
            m_Factory = ToDispose(m_SwapChain.GetParent<Factory>());
            m_Factory.MakeWindowAssociation(m_Form.Handle, WindowAssociationFlags.IgnoreAll);
        }

        private void DisposeBuffers()
        {
            m_DepthView.Dispose();
            m_DepthBuffer.Dispose();
            m_RenderView.Dispose();
            m_BackBuffer.Dispose();
        }

        private void RecreateBuffers()
        {
            Utilities.Dispose(ref m_BackBuffer);
            Utilities.Dispose(ref m_RenderView);
            Utilities.Dispose(ref m_DepthBuffer);
            Utilities.Dispose(ref m_DepthView);

            m_BackBuffer = Texture2D.FromSwapChain<Texture2D>(m_SwapChain, 0);
            m_RenderView = new RenderTargetView(m_Device, m_BackBuffer);
            m_DepthBuffer = new Texture2D(m_Device, new Texture2DDescription()
            {
                Format = Format.D32_Float,
                ArraySize = 1,
                MipLevels = 1,
                Width = Width,
                Height = Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });
            m_DepthView = new DepthStencilView(m_Device, m_DepthBuffer);

            m_Context.Rasterizer.SetViewport(new Viewport(0, 0, Width, Height, 0.0f, 1.0f));
            m_Context.OutputMerger.SetTargets(m_DepthView, m_RenderView);
        }

    }
}