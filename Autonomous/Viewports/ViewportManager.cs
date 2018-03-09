using System.Collections.Generic;
using System.Linq;
using Autonomous.Impl.GameObjects;

namespace Autonomous.Impl.Viewports
{
    class ViewportManager
    {
        private readonly ViewportFactory viewportFactory;

        public ViewportManager(ViewportFactory viewportFactory)
        {
            this.viewportFactory = viewportFactory;
        }

        public List<ViewportWrapper> Viewports { get; private set; }

        public void SetViewports(IEnumerable<GameObject> gameObjects)
        {
            SetViewports(gameObjects, CameraSetup.CameraDefault);
        }

        public void SetViewports(IEnumerable<GameObject> gameObjects, CameraSetup cameraSetup)
        {
            Viewports = viewportFactory.CreateViewPorts(gameObjects, cameraSetup).ToList();
        }
    }
}
