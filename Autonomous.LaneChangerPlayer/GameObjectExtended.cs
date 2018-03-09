using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autonomous.Public;

namespace Autonomous.LaneChangerPlayer
{
    public class GameObjectExtended : GameObjectState
    {
        public float CenterY => (BoundingBox.Top + BoundingBox.Bottom) / 2;

        public float DistanceToStop
        {
            get
            {
                if (VY == 0) return 0f;
                return 0.5f * VY * VY / MaximumDeceleration;
            }
        }

        public float EdgeOffsetY => BoundingBox.Height / 2;

        public GameObjectExtended(GameObjectState gameObjectState)
            : base(gameObjectState.Id,
                gameObjectState.GameObjectType,
                gameObjectState.BoundingBox,
                gameObjectState.VX, gameObjectState.VY,
                gameObjectState.Damage)
        {
        }

        public float GetDistance(GameObjectExtended other)
        {
            var distanceBetweenCenters = Math.Abs(CenterY - other.CenterY);
            return distanceBetweenCenters - EdgeOffsetY - other.EdgeOffsetY;
        }
    }
}