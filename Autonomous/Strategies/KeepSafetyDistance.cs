using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autonomous.Public;
using Autonomous.Impl.GameObjects;

namespace Autonomous.Impl.Strategies
{
    class KeepSafetyDistance : IControlStrategy
    {
        private IGameStateProvider _gameStateProvider;
        private float _desiredSpeed;

        public  GameObject GameObject { get; set; }
        public ControlState Calculate()
        {
            if (GameObject == null)
                throw new InvalidOperationException("GameObject not set");
            var go = GetClosestObjectInFront(_gameStateProvider.GameStateInternal.GameObjects);

            if (go != null)
            {
                float otherSpeed = go.OppositeDirection == GameObject.OppositeDirection ? go.VY : 0; //  -go.VY;
                float distanceToStop = CalculateDistanceToStop(otherSpeed, GameConstants.PlayerDeceleration);

                float selfDistancveToStop = CalculateDistanceToStop(GameObject.VY, GameConstants.PlayerDeceleration);
                float distanceBetweenCars = Math.Abs(GameObject.Y - go.Y) - GameObject.Height / 2 - go.Height / 2;
                float plusSafeDistance = 1 + GameObject.VY * 0.5f;
                if (distanceBetweenCars  < selfDistancveToStop - distanceToStop + plusSafeDistance)
                    return new ControlState() {Acceleration = -1};
            }

            if (Math.Abs(GameObject.VY - _desiredSpeed) < 5)
                return new ControlState() { Acceleration = 0 };

            if (GameObject.VY > _desiredSpeed)
                return new ControlState() { Acceleration = -1 };

            return new ControlState() { Acceleration = 0.5f };
        }

        public KeepSafetyDistance(float desiredSpeed, IGameStateProvider gameStateProvider )
        {
            _desiredSpeed = desiredSpeed;
            _gameStateProvider = gameStateProvider;
        }
        private float CalculateDistanceToStop(float v, float breakDeceleration)
        {
            return 0.5f * v * v / breakDeceleration;
        }

        private GameObject GetClosestObjectInFront(IEnumerable<GameObject> objects)
        {
            if (!GameObject.OppositeDirection)
                return objects.Where(o => o != GameObject && o.Y > GameObject.Y).OrderBy(o => o.Y).Where(IsOverlappingHorizontally).FirstOrDefault();
            else
                return objects.Where(o => o != GameObject && o.Y < GameObject.Y).OrderByDescending(o => o.Y).Where(IsOverlappingHorizontally).FirstOrDefault();
        }

        private bool IsOverlappingHorizontally(GameObject second)
        {
            var r1 = GameObject.BoundingBox;
            var r2 = second.BoundingBox;
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
