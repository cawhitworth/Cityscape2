using System;
using SharpDX;
using SharpDX.DirectInput;

namespace CityScape2
{
    class Camera
    {
        private readonly IInput m_Input;
        private Matrix m_Projection;
        private Matrix m_View;
        private Vector4 m_Position = new Vector4(0.0f, 5.0f, 5.0f, 1.0f);

        private float m_HAngle, m_VAngle = -0.25f;
        private long m_Last;

        public Camera(IInput input, int width, int height)
        {
            m_Input = input;
            SetProjection(width, height);
        }

        public Matrix View { get { return m_View; } } 
        public Matrix Projection { get { return m_Projection; } } 

        public void SetProjection(int width, int height)
        {
            m_Projection = Matrix.PerspectiveFovLH((float) Math.PI/4.0f, (float) width/height, 0.01f, 1000.0f);
        }

        public void Update(long t)
        {
            var elapsed = t - m_Last;
            m_Last = t;

            var mult = 1.0f;
            if (m_Input.IsKeyDown(Key.LeftShift))
                mult *= 2.0f;
            if (m_Input.IsKeyDown(Key.LeftControl))
                mult *= 2.0f;

            var mouseDelta = m_Input.MousePosition();

            m_HAngle += mouseDelta.X*(elapsed/5000.0f);
            m_VAngle -= mouseDelta.Y*(elapsed/5000.0f);

            if (m_VAngle < -1.57) m_VAngle = -1.57f;
            if (m_VAngle > 1.57) m_VAngle = 1.57f;

            var lookDir = new Vector4(0.0f, 0.0f, -1.0f, 0.0f);
            lookDir = Vector4.Transform(lookDir, Matrix.RotationX(m_VAngle));
            lookDir = Vector4.Transform(lookDir, Matrix.RotationY(m_HAngle));

            var lookDir3 = new Vector3(lookDir.X, lookDir.Y, lookDir.Z);
            var pos3 = new Vector3(m_Position.X, m_Position.Y, m_Position.Z);

            var lookAt = pos3 + lookDir3;

            var right = Vector3.Cross(new Vector3(0.0f, 1.0f, 0.0f), lookDir3);
            right.Normalize();

            var up = Vector3.Cross(lookDir3, right);

            up.Normalize();

            m_View = Matrix.LookAtLH(pos3, lookAt, up);

            if (m_Input.IsKeyDown(Key.W))
            {
                m_Position += lookDir*mult*(elapsed/1000.0f);
            }

            if (m_Input.IsKeyDown(Key.S))
            {
                m_Position -= lookDir*mult*(elapsed/1000.0f);
            }
            
            if (m_Input.IsKeyDown(Key.A))
            {
                m_Position -= new Vector4(right, 0.0f)*mult*(elapsed/1000.0f);
            }

            if (m_Input.IsKeyDown(Key.D))
            {
                m_Position += new Vector4(right, 0.0f)*mult*(elapsed/1000.0f);
            }
        }

    }
}