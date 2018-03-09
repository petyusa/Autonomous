using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Autonomous.Impl.GameObjects
{
    static class CollisionDetector
    {
        public static bool IsCollision(GameObject first, GameObject second)
        {
            var rect1 = GetRectangle(first);
            var rect2 = GetRectangle(second);
            return rect1.Intersects(rect2);
        }

        private static Rectangle GetRectangle(GameObject gameObject)
        {
            float x = gameObject.X - gameObject.Width / 2;
            float y = gameObject.Y - gameObject.Height / 2;

            return new Rectangle((int)(x*10000), (int)(y*10000), (int)(gameObject.Width*10000 * 0.95f), (int)(gameObject.Height*10000 * 0.95f));
        }
    }
}
