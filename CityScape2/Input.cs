using System;
using SharpDX;
using SharpDX.DirectInput;

namespace CityScape2
{
    class Input : IInput, IDisposable
    {
        private readonly DirectInput m_Input = new DirectInput();
        private readonly Mouse m_Mouse;
        private readonly Keyboard m_Keyboard;
        private KeyboardState m_KeyboardState;
        private MouseState m_MouseState;
        private Vector2 m_MousePosition;

        public Input(IntPtr windowHandle)
        {
            m_Mouse = new Mouse(m_Input);
            m_Keyboard = new Keyboard(m_Input);
            m_Keyboard.SetCooperativeLevel(windowHandle, CooperativeLevel.Foreground | CooperativeLevel.Exclusive);

            m_Mouse.Properties.AxisMode = DeviceAxisMode.Relative;
            m_Mouse.SetCooperativeLevel(windowHandle, CooperativeLevel.Foreground | CooperativeLevel.Exclusive);

        }

        public void Update()
        {
            try
            {
                m_KeyboardState = new KeyboardState();
                m_Keyboard.GetCurrentState(ref m_KeyboardState);
            }
            catch
            {
                m_KeyboardState = null;
            }

            if (m_KeyboardState == null)
                TryAcquireKeyboard();

            try
            {
                m_MouseState = new MouseState();
                m_Mouse.GetCurrentState(ref m_MouseState);
                m_MousePosition.X = m_MouseState.X;
                m_MousePosition.Y = m_MouseState.Y;
            }
            catch
            {
                m_MouseState = null;
            }

            if (m_MouseState == null)
                TryAcquireMouse();
        }

        private void TryAcquireKeyboard()
        {
            try
            {
                m_Keyboard.Acquire();
            }
            catch
            {
                m_KeyboardState = null;
            }
        }

        private void TryAcquireMouse()
        {
            try
            {
                m_Mouse.Acquire();
            }
            catch
            {
                m_MouseState = null;
            }
        }

        public bool IsKeyDown(Key key)
        {
            return m_KeyboardState != null && m_KeyboardState.IsPressed(key);
        }

        public bool IsKeyUp(Key key)
        {
            return m_KeyboardState != null && !m_KeyboardState.IsPressed(key);
        }

        public Vector2 MousePosition()
        {
            return m_MousePosition;
        }

        public void Dispose()
        {
            m_Mouse.Unacquire();
            m_Mouse.Dispose();
            
            m_Keyboard.Unacquire();
            m_Keyboard.Dispose();

            m_Input.Dispose();
        }
    }
}