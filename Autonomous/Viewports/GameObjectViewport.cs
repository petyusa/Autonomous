using Microsoft.Xna.Framework;
using Autonomous.Impl.GameObjects;
using System;

namespace Autonomous.Impl.Viewports
{
    class GameObjectViewport : ViewportWrapper
    {
        private CameraSetup cameraSetup;
        private GameObject _gameObject;
        private float offsetY;
        private float offsetZ;
        private float stepY;
        private float stepZ;

        public GameObjectViewport(int x, int y, int width, int height, 
            GameObject gameObject, CameraSetup cameraSetup, bool shadowEffectEnabled) 
            : base(x, y, width, height, gameObject, shadowEffectEnabled)
        {
            _gameObject = gameObject;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(30), (float)width / height, 0.1f, 500f);
            UseCameraSetup(cameraSetup);
        }

        public override void UseCameraSetup(CameraSetup setup)
        {
            cameraSetup = setup;
            offsetY = cameraSetup.CameraPositionStartY;
            offsetZ = cameraSetup.CameraPositionStartZ;
            stepY = (cameraSetup.CameraPositionEndY - cameraSetup.CameraPositionStartY) / 100;
            stepZ = (cameraSetup.CameraPositionEndZ - cameraSetup.CameraPositionStartZ) / 100;
        }

        protected override void UpdateCore()
        {
            CameraPosition = new Vector3(_gameObject.X, offsetY, -_gameObject.Y + offsetZ);
            LookAt = new Vector3(0, Math.Min(0, -100 + (offsetZ / 100)), -99999);

            CameraOrientation = Vector3.UnitY;
            offsetY = Math.Min(cameraSetup.CameraPositionEndY, offsetY + stepY);
            offsetZ = Math.Min(cameraSetup.CameraPositionEndZ, offsetZ + stepZ);
        }
    }
}
