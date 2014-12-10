using System;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;

using Device=SharpDX.Direct3D11.Device;

namespace CityScape2
{
    internal static class Program
    {

        [STAThread]
        static void Main()
        {

            var form = new RenderForm();

            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                IsWindowed = true,
                ModeDescription =
                    new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1),
                        Format.R8G8B8A8_UNorm),
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            // Create device + swapchain
            Device device;
            SwapChain swapChain;
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device, out swapChain);
            var ctx = device.ImmediateContext;

            // Ignore Windows events
            var factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

            form.KeyUp += (sender, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                    form.Close();
            };

            var backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            var renderView = new RenderTargetView(device, backBuffer);
            var depthBuffer = new Texture2D(device, new Texture2DDescription()
            {
                Format = Format.D32_Float,
                ArraySize = 1,
                MipLevels = 1,
                Width = form.ClientSize.Width,
                Height = form.ClientSize.Height,
                SampleDescription = new SampleDescription(1,0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });
            var depthView = new DepthStencilView(device, depthBuffer);

            RenderLoop.Run(form, () =>
            {
                ctx.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
                ctx.ClearRenderTargetView(renderView, Color.CornflowerBlue);

                swapChain.Present(0, PresentFlags.None);
            });
        }
    }
}
