using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autonomous.Public;

namespace Autonomous.LaneChangerPlayer
{
    public class LaneObject
    {
        private readonly uint _priority;
        public byte Id { get; }
        public float X { get; set; }

        public uint GetScore(IEnumerable<GameObjectState> gameObjects,
            GameObjectExtended self,
            byte currentLane)
        {
            return _priority
                   + GetDistanceScore(currentLane)
                   + GetTrafficScore(gameObjects, self)
                   + GetVelocityScore();
        }

        private uint GetDistanceScore(byte currentLane)
        {
            return (uint) Math.Abs(Id - currentLane) * 2;
        }

        private uint GetTrafficScore(IEnumerable<GameObjectState> gameObjects,
            GameObjectExtended self)
        {
            var closestObject = GetClosestObjectInFront(gameObjects, self);
            if (closestObject == null) return 0;
            var closest = new GameObjectExtended(closestObject);
            var score = Math.Min(500, Math.Exp(self.GetDistance(closest) - 50)); // get exp
            score = Math.Max(0, 500 - score); // reverse
            //score = score / 10;
            return (uint) score;
        }

        private uint GetVelocityScore()
        {
            return 0;
        }

        public LaneObject(byte id, float x, uint priority)
        {
            Id = id;
            X = x;
            _priority = priority;
        }

        public GameObjectState GetClosestObjectInFront(IEnumerable<GameObjectState> gameObjects,
            GameObjectState self)
        {
            var laneBound = new RectangleF(X, self.BoundingBox.Y, self.BoundingBox.Width, self.BoundingBox.Height);
            return gameObjects
                .Where(gameObject => gameObject.BoundingBox.Y > laneBound.Y)
                .FirstOrDefault(o => IsOverlappingHorizontally(laneBound, o.BoundingBox));
        }

        private static bool IsOverlappingHorizontally(RectangleF r1, RectangleF r2)
        {
            return Between(r1.Left, r1.Right, r2.Left) ||
                   Between(r1.Left, r1.Right, r2.Right) ||
                   Between(r2.Left, r2.Right, r1.Right) ||
                   Between(r2.Left, r2.Right, r1.Left);
        }

        private static bool Between(float limit1, float limit2, float value)
        {
            return (value >= limit1 && value <= limit2) || (value >= limit2 && value <= limit1);
        }
    }
}