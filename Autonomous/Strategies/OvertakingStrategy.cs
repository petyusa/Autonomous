using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autonomous.Public;
using Autonomous.Impl.GameObjects;

namespace Autonomous.Impl.Strategies
{
    internal class LaneInfo
    {
        public bool IsInLane { get; set; }
        public bool IsInBrakeZone { get; set; }
        public bool IsInCrashZone { get; set; }

        public bool IsFrontCarOverlapping { get; set; }
        public bool IsBackCarInCrashZone { get; set; }
        public float CenterX;
        public float FrontCarSpeed { get; set; }
    }

    class OvertakingStrategy : IControlStrategy
    {
        protected IGameStateProvider _gameStateProvider;
        private float _desiredSpeed;
        private int _preferredLane;

        public OvertakingStrategy(float desiredSpeed, int preferredLane, IGameStateProvider gameStateProvider)
        {
            _desiredSpeed = desiredSpeed;
            _gameStateProvider = gameStateProvider;
            _preferredLane = preferredLane;
        }

        public GameObject GameObject { get; set; }
        public virtual ControlState Calculate()
        {
            if (GameObject == null)
                throw new InvalidOperationException("GameObject not set");

            int selfIndex = _gameStateProvider.GameStateInternal.GameObjects.IndexOf(GameObject);
            LaneInfo[] laneInfos = new LaneInfo[2] {GetLaneInfo(0, selfIndex), GetLaneInfo(1, selfIndex)};
            int targetLane = GetTargetLane(laneInfos);

            return GetControlState(targetLane, laneInfos);
        }

        protected ControlState GetControlState(int targetLane, LaneInfo[] lanes)
        {
            float targetSpeed = targetLane == _preferredLane ? _desiredSpeed : _desiredSpeed * 1.1f;
            bool brake = (lanes[targetLane].IsInBrakeZone && lanes[targetLane].FrontCarSpeed < GameObject.VY) || 
                (lanes[0].IsInLane && lanes[0].IsInCrashZone && lanes[0].IsFrontCarOverlapping) || 
                (lanes[1].IsInLane && lanes[1].IsInCrashZone && lanes[1].IsFrontCarOverlapping);
            float acceleration = 0.0f;
            if ((brake && GameObject.VY > 0) || GameObject.VY > targetSpeed)
                acceleration = -1f;
            else if (Math.Abs(GameObject.VY - targetSpeed) < 2)
                acceleration = 0f;
            else
                acceleration = 1f;

            float vx = 0;
            if (Math.Abs(lanes[targetLane].CenterX - GameObject.X) < 0.2f)
                vx = 0;
            else if (lanes[targetLane].CenterX < GameObject.X)
                vx = -1;
            else
                vx = 1;

            if (GameObject.OppositeDirection)
                vx = -vx;

            return new ControlState() {Acceleration = acceleration, HorizontalSpeed = vx};
        }

        private LaneInfo GetLaneInfo(int laneIndex, int selfIndex)      
        {
            float width = GameConstants.LaneWidth*0.9f;
            float centerX = laneIndex == 0 ? width * 1.6f : width / 2;

            if (GameObject.OppositeDirection)
                centerX = -centerX;

            var carFront = GetClosestObjectInLaneFrontFast(_gameStateProvider.GameStateInternal.GameObjects, laneIndex, selfIndex);
            var carBack = GetClosestObjectInLaneBackFast(_gameStateProvider.GameStateInternal.GameObjects, laneIndex, selfIndex);

            float selfDistancveToStop = CalculateDistanceToStop(GameObject.VY, GameConstants.PlayerDeceleration);
            bool frontBrake = false;
            bool frontCrash = false;
            bool isCarOverlapping = false;

            if (carFront != null)
            {
                float otherSpeed = carFront.OppositeDirection == GameObject.OppositeDirection ? carFront.VY : 0; //  -go.VY;
                float distanceToStop = CalculateDistanceToStop(otherSpeed, GameConstants.PlayerDeceleration);

                float distanceBetweenCars = Math.Abs(GameObject.Y - carFront.Y) - GameObject.Height / 2 - carFront.Height / 2;

                if (distanceBetweenCars < selfDistancveToStop - distanceToStop)
                    frontCrash = true;

                float plusSafeDistance = 20f + GameObject.VY-carFront.VY;
                if (distanceBetweenCars < selfDistancveToStop - distanceToStop + plusSafeDistance)
                    frontBrake = true;

                isCarOverlapping = IsOverlappingHorizontally(carFront);
            }

            bool backCrash = false;
            if (carBack != null)
            {
                float otherSpeed = carBack.OppositeDirection == GameObject.OppositeDirection
                    ? Math.Min(carBack.VY, 130f / 3.6f)
                    : 0; //  -go.VY;

                if (Math.Abs(GameObject.Y - carBack.Y) < GameObject.Height/2 + carBack.Height /2 + 1)
                    backCrash = true;
                else if (otherSpeed < GameObject.VY)
                    backCrash = false;
                else
                { 

                    
                    float distanceToStop = CalculateDistanceToStop(otherSpeed-GameObject.VY, GameConstants.PlayerDeceleration);

                    float distanceBetweenCars =
                        Math.Abs(GameObject.Y - carBack.Y) - GameObject.Height / 2 - carBack.Height / 2;

                    if (distanceBetweenCars < distanceToStop)
                        backCrash = true;
                }
            }

            return new LaneInfo()
            {
                CenterX = centerX,
                IsBackCarInCrashZone = backCrash,
                IsInBrakeZone = frontBrake,
                IsInCrashZone = frontCrash,
                IsInLane = IsInLane(laneIndex, GameObject),
                IsFrontCarOverlapping = isCarOverlapping,
                FrontCarSpeed = carFront?.VY ?? float.NaN
            };
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

        private int GetTargetLane(LaneInfo[] laneInfos)
        {
            var preferredLaneInfo = laneInfos[_preferredLane];
            var otherLaneInfo = laneInfos[1 - _preferredLane];
            var otherLane = 1 - _preferredLane;

            if (!preferredLaneInfo.IsInBrakeZone && preferredLaneInfo.IsInLane)
                return _preferredLane;

            if ((otherLaneInfo.IsBackCarInCrashZone || otherLaneInfo.IsInCrashZone) && !otherLaneInfo.IsInLane)
                return _preferredLane;

            if ((preferredLaneInfo.IsBackCarInCrashZone || preferredLaneInfo.IsInCrashZone) && !preferredLaneInfo.IsInLane)
                return otherLane;

            if (!preferredLaneInfo.IsInBrakeZone)
                return _preferredLane;

            if (!otherLaneInfo.IsInBrakeZone)
                return otherLane;

            if (otherLaneInfo.IsInCrashZone)
                return _preferredLane;

            if (preferredLaneInfo.IsInCrashZone)
                return otherLane;

            if (preferredLaneInfo.IsInBrakeZone && otherLaneInfo.IsInBrakeZone && preferredLaneInfo.FrontCarSpeed >= otherLaneInfo.FrontCarSpeed)
                return _preferredLane;

            if (preferredLaneInfo.IsInBrakeZone && otherLaneInfo.IsInBrakeZone &&
                otherLaneInfo.FrontCarSpeed >= preferredLaneInfo.FrontCarSpeed)
                return otherLane;

                throw  new InvalidOperationException("State is not handled");

        }

        protected float CalculateDistanceToStop(float v, float breakDeceleration)
        {
            if (v == 0) return 0;
            return 0.5f * v * v / breakDeceleration;
        }

        private GameObject GetClosestObjectInLaneFrontFast(IList<GameObject> objects, int lane, int selfIndex)
        {
            return GetClosestObjectInLane(objects, lane, selfIndex, !GameObject.OppositeDirection);
        }

        private GameObject GetClosestObjectInLaneBackFast(IList<GameObject> objects, int lane, int selfIndex)
        {
            return GetClosestObjectInLane(objects, lane, selfIndex, GameObject.OppositeDirection);
        }

        private GameObject GetClosestObjectInLane(IList<GameObject> objects, int lane, int selfIndex, bool front)
        {
            if (front)
            {
                for (int i = selfIndex + 1; i < objects.Count; i++)
                {
                    if (Math.Abs(GameObject.Y - objects[i].Y) > 300)
                        return null;

                    if (IsInLane(lane, objects[i]))
                        return objects[i];
                }
            }
            else
            {
                for (int i = selfIndex - 1; i >= 0; i--)
                {
                    if (Math.Abs(GameObject.Y - objects[i].Y) > 300)
                        return null;

                    if (IsInLane(lane, objects[i]))
                        return objects[i];
                }
            }

            return null;
        }

        protected bool IsInLane(int laneIndex, GameObject second)
        {
            float min = laneIndex == 0 ? GameConstants.LaneWidth*1.1f : GameConstants.LaneWidth*0.1f;
            float max = laneIndex == 0 ? GameConstants.LaneWidth*1.9f : GameConstants.LaneWidth*0.9f;

            if (GameObject.OppositeDirection)
            {
                max = -max;
                min = -min;
            }

            var r2 = second.BoundingBox;
            return Between(min, max, r2.Left) ||
                   Between(min, max, r2.Right) ||
                   Between(r2.Left, r2.Right, min) ||
                   Between(r2.Left, r2.Right, max);
        }

        private static bool Between(float limit1, float limit2, float value)
        {
            return (value >= limit1 && value <= limit2) || (value >= limit2 && value <= limit1);
        }
    }
}
