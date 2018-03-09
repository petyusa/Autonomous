using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Autonomous.Impl.GameObjects;
using Autonomous.Impl.Viewports;

namespace Autonomous.Impl
{
    class ViewportFactory
    {
        private readonly GraphicsDeviceManager graphics;

        public ViewportFactory(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
        }

        public IEnumerable<ViewportWrapper> CreateViewPorts(IEnumerable<GameObject> objects, CameraSetup cameraSetup)
        {
            var numViewPorts = objects.Count();
            if (numViewPorts < 1)
                throw new InvalidOperationException("There is no player");

            int sideViewWidth = graphics.PreferredBackBufferWidth / 10;
            int playerViewportWidth = graphics.PreferredBackBufferWidth - sideViewWidth;

            if (numViewPorts == 1)
            {
                int width = playerViewportWidth;
                int height = (int) graphics.PreferredBackBufferHeight;
                yield return CreateViewPort(objects.ElementAt(0), 0, 0, width, height, cameraSetup, true);
            }

            if (numViewPorts == 2)
            {
                int width = playerViewportWidth;
                int height = (int)graphics.PreferredBackBufferHeight /2;
                yield return CreateViewPort(objects.ElementAt(0), 0, 0, width, height, cameraSetup, true);
                yield return CreateViewPort(objects.ElementAt(1), 0, height, width, height, cameraSetup, true);
            }

            if (numViewPorts >= 3)
            {
                int width = playerViewportWidth / 2;
                int height = (int)graphics.PreferredBackBufferHeight / 2;
                yield return CreateViewPort(objects.ElementAt(0), 0, 0, width, height, cameraSetup, false);
                yield return CreateViewPort(objects.ElementAt(1), 0, height, width, height, cameraSetup, false);
                yield return CreateViewPort(objects.ElementAt(2), width, 0, width, height, cameraSetup, false);
                if (objects.Count()>3)
                    yield return CreateViewPort(objects.ElementAt(3), width, height, width, height, cameraSetup, false);
            }
            yield return new BirdsEyeViewport(playerViewportWidth, 0, sideViewWidth, graphics.PreferredBackBufferHeight, objects);

        }

        static ViewportWrapper CreateViewPort(GameObject gameObject, int x, int y, int width, int height, 
            CameraSetup cameraSetup, bool shadowEffectEnabled)
        {
            if (gameObject is FinishLine)
                return new FinishLineViewport(x, y, width, height, gameObject);
            return new GameObjectViewport(x, y, width, height, gameObject, cameraSetup, shadowEffectEnabled);
        }
    }
}
