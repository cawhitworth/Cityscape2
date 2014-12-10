using System.IO;
using System.Windows.Forms;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
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

            RenderLoop.Run(m_Form, () =>
            {
                if (recreate)
                {
                    RecreateBuffers();
                }

                m_Context.ClearDepthStencilView(m_DepthView, DepthStencilClearFlags.Depth, 1.0f, 0);
                m_Context.ClearRenderTargetView(m_RenderView, Color.CornflowerBlue);

                m_SwapChain.Present(0, PresentFlags.None);
            });

            DisposeBuffers();
            Dispose();
        }

        private void LoadShaders()
        {
            var vertShaderBytecode = File.ReadAllBytes("VertexShader.cso");
            m_VertexShader = ToDispose(new VertexShader(m_Device, vertShaderBytecode));

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
        }

    }
}