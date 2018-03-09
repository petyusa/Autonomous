using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Autonomous.Impl.GameObjects;

namespace Autonomous.Impl.Viewports
{
    class BirdsEyeViewport : ViewportWrapper
    {
        private IEnumerable<GameObject> _gameObjects;

        public BirdsEyeViewport(int x, int y, int width, int height, IEnumerable<GameObject> gameObjects) 
            : base(x, y, width, height, null, false)
        {
            _gameObjects = gameObjects;
            float w = 20f;
            Projection = Matrix.CreateOrthographic(w, w*height/width, 0.1f, 500f);
        }

        protected override void UpdateCore()
        {
            var firstY = _gameObjects.OrderByDescending(g => g.Y).First().Y;
            CameraPosition = new Vector3(0, 40, -firstY+20);
            LookAt = new Vector3(0, 0, -firstY+20);
            CameraOrientation = -Vector3.UnitZ;
        }
    }
}
