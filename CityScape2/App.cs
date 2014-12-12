using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Buffer = SharpDX.Direct3D11.Buffer;
using Color = SharpDX.Color;
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
        private Matrix m_Proj;

        public void Run()
        {
            CreateDeviceAndSwapChain();

            m_Form.KeyUp += (sender, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                    m_Form.Close();
            };

            var recreate = true;
            m_Form.Resize += (sender, args) => recreate = true;

            RecreateBuffers();


            var city = new City(m_Device, m_Context);

            var view = Matrix.LookAtLH(new Vector3(0, 7.5f, -40), new Vector3(0, 0, 0), Vector3.UnitY);
            view.Transpose();
            var proj = Matrix.Identity;

            var clock = new Stopwatch();
            clock.Start();
            var overlay = new Overlay(clock.ElapsedMilliseconds);

            var clearColor = new Color(0.1f, 0.1f, 0.2f, 0.0f);
            RenderLoop.Run(m_Form, () =>
            {
                if (recreate)
                {
                    RecreateBuffers();
                    proj = Matrix.PerspectiveFovLH((float) Math.PI/4.0f, (float) Width/Height, 0.01f, 1000.0f);
                    proj.Transpose();
                    recreate = false;
                }


                m_Context.ClearDepthStencilView(m_DepthView, DepthStencilClearFlags.Depth, 1.0f, 0);
                m_Context.ClearRenderTargetView(m_RenderView, clearColor);

                var polys = city.Draw(clock.ElapsedMilliseconds, view, proj);
                overlay.Draw(clock.ElapsedMilliseconds, polys);

                m_SwapChain.Present(0, PresentFlags.None);
            });

            DisposeBuffers();
            Dispose();
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

            m_Form.Width = 1280;
            m_Form.Height = 800;

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

            m_Context.Rasterizer.State = new RasterizerState(m_Device, new RasterizerStateDescription(
                )
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.Back,
                IsFrontCounterClockwise = false,
                DepthBias = 0,
                DepthBiasClamp = 0,
                SlopeScaledDepthBias = 0,
                IsDepthClipEnabled = false,
                IsScissorEnabled = false,
                IsMultisampleEnabled = false,
                IsAntialiasedLineEnabled = false

            });
            m_Context.Rasterizer.SetViewport(new Viewport(0, 0, Width, Height, 0.0f, 1.0f));
            m_Context.OutputMerger.SetTargets(m_DepthView, m_RenderView);
        }

    }
}