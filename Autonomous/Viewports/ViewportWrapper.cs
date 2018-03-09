using Autonomous.Impl.GameObjects;
using Autonomous.Impl.Viewports;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Autonomous.Impl.Viewports
{
    public abstract class ViewportWrapper
    {
        public ViewportWrapper(int x, int y, int width, int height, GameObject gameObject, bool shadowEffectEnabled)
        {
            Viewport = new Viewport()
            {
                X = x,
                Y = y,
                Width = width,
                Height = height,
                MinDepth = 0,
                MaxDepth = 1
            };
            GameObject = gameObject;
            ShadowEffectEnabled = shadowEffectEnabled;
        }
        public Viewport Viewport { get; private set; }

        public Matrix View { get; private set; }

        public Vector3 CameraPosition { get; protected set; }

        public Vector3 LookAt { get; protected set; }

        public Vector3 CameraOrientation { get; protected set; }

        public Matrix Projection { get; protected set; }

        public GameObject GameObject { get; }

        public bool ShadowEffectEnabled { get; }

        protected abstract void UpdateCore();

        public void Update()
        {
            UpdateCore();
            View = Matrix.CreateLookAt(CameraPosition, LookAt, CameraOrientation);            
        }

        public virtual void UseCameraSetup(CameraSetup setup)
        {

        }
    }
}
