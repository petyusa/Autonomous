using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Autonomous.Public;
using Microsoft.Xna.Framework.Input;

namespace Autonomous.CleverPlayer
{
    [Export(typeof(IPlayer))]
    [ExportMetadata("PlayerName", "NotSoCleverPlayer")]
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public class CleverPlayer : IPlayer
    {
        private string _playerId;
        private float _desiredLane;
        private float _currentLane;
        private float _myPositionY;
        private float _myPositionX;
        private float _changeLaneDistance;
        private GameObjectState _myCar;
        private List<GameObjectState> _objectsNextToMe;

        public void Finish()
        {
        }

        public void Initialize(string playerId)
        {
            _playerId = playerId;
        }

        public PlayerAction Update(GameState gameState)
        {
            var acceleration = 1;

            SetMyCarInfo(gameState);
            var objectsInFront = GetObjectsInFront(gameState);
            var closestObjectInMyLane = GetClosestObjectInMyLane(objectsInFront);
            if (closestObjectInMyLane == null)
                return GoToDesiredLane(acceleration);

            GameObjectState closestObjectInLeftLane;
            GameObjectState closestObjectInRightLane;
            bool shouldChangeLane;
            switch (_currentLane)
            {
                case Lane.RightRight:
                    closestObjectInLeftLane = GetClosestObjectInLeftLane(objectsInFront);
                    shouldChangeLane = ShouldChangeLane(closestObjectInMyLane, closestObjectInLeftLane);
                    if (ShouldBrake(closestObjectInMyLane, closestObjectInLeftLane, shouldChangeLane))
                        acceleration = -1;
                    break;
                case Lane.Right:
                    closestObjectInRightLane = GetClosestObjectInRightLane(objectsInFront);
                    closestObjectInLeftLane = GetClosestObjectInLeftLane(objectsInFront);
                    shouldChangeLane = ShouldChangeLane(closestObjectInMyLane, closestObjectInLeftLane, closestObjectInRightLane);
                    if (ShouldBrake(closestObjectInMyLane, closestObjectInLeftLane, closestObjectInRightLane, shouldChangeLane))
                        acceleration = -1;
                    break;
                case Lane.Left:
                    closestObjectInRightLane = GetClosestObjectInRightLane(objectsInFront);
                    closestObjectInLeftLane = GetClosestObjectInLeftLane(objectsInFront);
                    shouldChangeLane = ShouldChangeLane(closestObjectInMyLane, closestObjectInLeftLane, closestObjectInRightLane);
                    if (ShouldBrake(closestObjectInMyLane, closestObjectInLeftLane, closestObjectInRightLane, shouldChangeLane))
                        acceleration = -1;
                    break;
                case Lane.LeftLeft:
                    closestObjectInRightLane = GetClosestObjectInRightLane(objectsInFront);
                    shouldChangeLane = ShouldChangeLane(closestObjectInMyLane, closestObjectInRightLane);
                    if (ShouldBrake(closestObjectInMyLane, closestObjectInRightLane, shouldChangeLane))
                        acceleration = -1;
                    break;
            }

            return GoToDesiredLane(acceleration);
        }

        private List<GameObjectState> GetObjectsInFront(GameState gameState)
        {
            if (gameState.PlayerCollision)
            {
                return gameState.GameObjectStates
                .Where(g => g.GameObjectType != GameObjectType.BusStop &&
                            g.GameObjectType != GameObjectType.FinishLine &&
                            g.BoundingBox.Bottom > _myPositionY &&
                            g.BoundingBox.Bottom - _myPositionY < 200
                ).ToList();
            }

            return gameState.GameObjectStates
                .Where(g => g.GameObjectType != GameObjectType.Player &&
                            g.GameObjectType != GameObjectType.BusStop &&
                            g.GameObjectType != GameObjectType.FinishLine &&
                            g.BoundingBox.Bottom > _myPositionY &&
                            g.BoundingBox.Bottom - _myPositionY < 200
                ).ToList();
        }

        private List<GameObjectState> GetObjectsNextToMe(GameState gameState)
        {
            if (gameState.PlayerCollision)
            {
                return gameState.GameObjectStates
                    .Where(g => g.GameObjectType != GameObjectType.BusStop &&
                                g.GameObjectType != GameObjectType.FinishLine &&
                                (g.BoundingBox.Bottom.IsBetween(_myCar.BoundingBox.Top, _myCar.BoundingBox.Bottom) ||
                                 g.BoundingBox.Top.IsBetween(_myCar.BoundingBox.Top, _myCar.BoundingBox.Bottom))
                    ).ToList();
            }

            return gameState.GameObjectStates
                .Where(g => g.GameObjectType != GameObjectType.BusStop &&
                            g.GameObjectType != GameObjectType.Player &&
                            g.GameObjectType != GameObjectType.FinishLine &&
                            (g.BoundingBox.Bottom.IsBetween(_myCar.BoundingBox.Top, _myCar.BoundingBox.Bottom) ||
                             g.BoundingBox.Top.IsBetween(_myCar.BoundingBox.Top, _myCar.BoundingBox.Bottom))
                ).ToList();
        }


        private static float GetDistanceFromObject(float myPositionY, GameObjectState closestObject)
        {
            if (closestObject != null)
                return closestObject.BoundingBox.Top - myPositionY;
            return 100000f;
        }

        private GameObjectState GetClosestObjectInRightLane(IEnumerable<GameObjectState> otherObjectsInFront)
        {
            return otherObjectsInFront
                .FirstOrDefault(o => o.GetLane() == _currentLane + GameConstants.LaneWidth);
        }

        private GameObjectState GetClosestObjectInLeftLane(IEnumerable<GameObjectState> otherObjectsInFront)
        {
            return otherObjectsInFront
                .FirstOrDefault(o => o.GetLane() == _currentLane - GameConstants.LaneWidth);
        }

        private GameObjectState GetClosestObjectInMyLane(List<GameObjectState> otherObjectsInFront)
        {
            return otherObjectsInFront
                .FirstOrDefault(o => o.GetLane() == _currentLane &&
                            o.BoundingBox.CenterY - _myPositionY < 100);
        }

        private float CalculateDistanceToStop(float speed, float breakDeceleration)
        {
            if (speed == 0)
                return 0f;
            return 0.5f * speed * speed / breakDeceleration;
        }

        private float CalculateDistanceToChangeLane(float speed)
        {
            var timeToChangeLane = GameConstants.LaneWidth / GameConstants.PlayerHoriztontalSpeed;
            return timeToChangeLane * speed;
        }

        private bool ShouldChangeLane(GameObjectState objectInMyLane, GameObjectState objectInOtherLane)
        {
            var distanceInMyLane = GetDistanceFromObject(_myPositionY, objectInMyLane);
            var distanceInOtherLane = GetDistanceFromObject(_myPositionY, objectInOtherLane);
            if (distanceInMyLane > distanceInOtherLane)
                return false;

            var distanceToStop = CalculateDistanceToStop(_myCar.VY, _myCar.MaximumDeceleration);
            switch (_currentLane)
            {
                case Lane.RightRight:
                    if (!CanChangeLane(Lane.Right))
                        return false;
                    if (objectInOtherLane == null)
                    {
                        _desiredLane = Lane.Right;
                        return true;
                    }

                    if (objectInOtherLane.GameObjectType == GameObjectType.Roadblock &&
                        distanceInOtherLane < distanceToStop)
                        return false;

                    _desiredLane = Lane.Right;
                    return true;
                case Lane.LeftLeft:
                    if (!CanChangeLane(Lane.Left))
                        return false;
                    if (objectInOtherLane == null)
                    {
                        _desiredLane = Lane.Left;
                        return true;
                    }

                    if (objectInOtherLane.GameObjectType == GameObjectType.Roadblock &&
                        distanceInOtherLane < distanceToStop)
                        return false;

                    _desiredLane = Lane.Left;
                    return true;
            }
            return true;
        }

        private bool ShouldChangeLane(GameObjectState objectInMyLane, GameObjectState objectInLeftLane, GameObjectState objectInRightLane)
        {
            if (objectInMyLane == null)
                return false;

            var distanceToStop = CalculateDistanceToStop(_myCar.VY, _myCar.MaximumDeceleration);
            var distanceFromLeftObjectAfterLangeChange = CalculateDistanceAfterLaneChange(objectInLeftLane);
            var distanceFromRightObjectAfterLangeChange = CalculateDistanceAfterLaneChange(objectInRightLane);

            if (distanceFromRightObjectAfterLangeChange > distanceFromLeftObjectAfterLangeChange)
            {
                switch (_currentLane)
                {
                    case Lane.Right:
                        if (!CanChangeLane(Lane.RightRight))
                            return false;
                        if (objectInRightLane == null)
                        {
                            _desiredLane = Lane.RightRight;
                            return true;
                        }

                        if (objectInRightLane.GameObjectType == GameObjectType.Roadblock &&
                            distanceFromRightObjectAfterLangeChange < distanceToStop)
                            return false;

                        _desiredLane = Lane.RightRight;
                        return true;
                    case Lane.Left:
                        if (!CanChangeLane(Lane.Right))
                            return false;

                        if (objectInRightLane == null)
                        {
                            _desiredLane = Lane.Right;
                            return true;
                        }

                        if (objectInRightLane.GameObjectType == GameObjectType.Roadblock &&
                            distanceFromRightObjectAfterLangeChange < distanceToStop)
                            return false;

                        _desiredLane = Lane.Right;
                        return true;
                    default:
                        return false;
                }
            }
            switch (_currentLane)
            {
                case Lane.Right:
                    if (!CanChangeLane(Lane.Left))
                        return false;
                    if (objectInLeftLane == null)
                    {
                        _desiredLane = Lane.Left;
                        return true;
                    }

                    if (distanceFromLeftObjectAfterLangeChange < _changeLaneDistance * 2)
                        return false;

                    if (objectInLeftLane.GameObjectType == GameObjectType.Roadblock &&
                        distanceFromLeftObjectAfterLangeChange < distanceToStop)
                        return false;

                    _desiredLane = Lane.Left;
                    return true;
                case Lane.Left:
                    if (!CanChangeLane(Lane.LeftLeft))
                        return false;

                    if (objectInLeftLane == null)
                    {
                        _desiredLane = Lane.LeftLeft;
                        return true;
                    }

                    if (objectInLeftLane.GameObjectType == GameObjectType.Roadblock &&
                        distanceFromLeftObjectAfterLangeChange < distanceToStop)
                        return false;

                    _desiredLane = Lane.LeftLeft;
                    return true;
                default:
                    return false;
            }
        }

        private bool ShouldBrake(GameObjectState objectInMyLane, GameObjectState objectInOtherLane, bool shouldChangeLane)
        {
            if (objectInMyLane == null)
                return false;
            float distance;
            var distanceToStop = CalculateDistanceToStop(_myCar.VY, _myCar.MaximumDeceleration);
            if (!shouldChangeLane)
            {
                if (objectInOtherLane == null)
                    return true;
                var closestObjectInMyLaneBottom = objectInMyLane.BoundingBox.Top;
                var closestObjectInOtherLaneBottom = objectInOtherLane.BoundingBox.Top;
                if (closestObjectInMyLaneBottom > closestObjectInOtherLaneBottom + _changeLaneDistance)
                    return false;
                distance = GetDistanceFromObject(_myPositionY, objectInMyLane);
                return distance < distanceToStop;
            }

            if (objectInOtherLane == null)
                return false;

            distance = GetDistanceFromObject(_myPositionY, objectInOtherLane);
            return distance < distanceToStop;
        }

        private float CalculateDistanceAfterLaneChange(GameObjectState objectInFront)
        {
            if (objectInFront == null)
                return 10000;
            var myPositionAfterLaneChange = _myPositionY + _changeLaneDistance;
            var objectInfrontPositionAfterLaneChange =
                objectInFront.BoundingBox.Top + CalculateDistanceToChangeLane(objectInFront.VY);
            return objectInfrontPositionAfterLaneChange - myPositionAfterLaneChange;
        }

        private bool ShouldBrake(GameObjectState objectInMyLane, GameObjectState objectInLeftLane, GameObjectState objectInRightLane, bool shouldChangeLane)
        {
            if (objectInMyLane == null)
                return false;

            var distanceToStop = CalculateDistanceToStop(_myCar.VY, _myCar.MaximumDeceleration);
            float distance;
            if (!shouldChangeLane)
            {
                distance = GetDistanceFromObject(_myPositionY, objectInMyLane);
                if (objectInMyLane.VY < _myCar.VY)
                    return distance < distanceToStop;
                return false;
            }

            var closestObjectInMyLaneBottom = objectInMyLane.BoundingBox.Top;
            if (_desiredLane == _currentLane + GameConstants.LaneWidth)
            {
                if (objectInRightLane == null)
                    return false;

                distance = GetDistanceFromObject(_myPositionY, objectInRightLane);
                if (objectInMyLane.VY < _myCar.VY)
                    return distance < distanceToStop;
                return false;
            }

            if (objectInLeftLane == null)
                return false;

            distance = GetDistanceFromObject(_myPositionY, objectInLeftLane);
            if (objectInMyLane.VY < _myCar.VY)
                return distance < distanceToStop;
            return false;
        }

        private bool CanChangeLane(float lane)
        {
            var objectNextToMe = _objectsNextToMe.FirstOrDefault(o => o.GetLane() == lane);
            return objectNextToMe == null;
        }

        private void SetMyCarInfo(GameState gameState)
        {
            _myCar = gameState.GameObjectStates.First(o => o.Id == _playerId);
            _currentLane = _myCar.GetLane();
            _myPositionX = _myCar.BoundingBox.CenterX;
            _myPositionY = _myCar.BoundingBox.Bottom;
            _desiredLane = _currentLane;
            _changeLaneDistance = CalculateDistanceToChangeLane(_myCar.VY);
            _objectsNextToMe = GetObjectsNextToMe(gameState);
        }

        private PlayerAction GoToDesiredLane(float acceleration)
        {
            var moveLeft = _desiredLane < _myPositionX && _myPositionX - _desiredLane > 0.2;
            var moveRight = _desiredLane > _myPositionX && _desiredLane - _myPositionX > 0.2;

            return new PlayerAction
            {
                MoveLeft = moveLeft,
                MoveRight = moveRight,
                Acceleration = acceleration
            };
        }
    }
}
