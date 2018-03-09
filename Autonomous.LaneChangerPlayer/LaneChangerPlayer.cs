using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using Autonomous.Public;

namespace Autonomous.LaneChangerPlayer
{
    [Export(typeof(IPlayer))]
    [ExportMetadata("PlayerName", "NotSoCleverPlayer")]
    public class LaneChangerPlayer : IPlayer
    {
        private string _playerId;
        private byte _currentLaneId = 2;
        private bool _isChangingLanes = false;
        private const float _laneOffset = GameConstants.RoadWidth / 2;

        private readonly LaneObject[] _lanes = new[]
        {
            new LaneObject(0, -4.5f, 6),
            new LaneObject(1, -1.5f, 5),
            new LaneObject(2, 1.5f, 1),
            new LaneObject(3, 4.5f, 2)
        };

        public void Finish()
        {
        }

        public void Initialize(string playerId)
        {
            _playerId = playerId;
        }

        public PlayerAction Update(GameState gameState)
        {
            /* TODO:
             * always overtake roadblocks
             */
            float accelerationY = 1;
            bool left = false, right = false;

            var selfObject = gameState
                .GameObjectStates
                .Single(o => o.Id == _playerId);
            var self = new GameObjectExtended(selfObject);

            var allObstacleObjects = gameState
                .GameObjectStates
                .Where(g => g.GameObjectType == GameObjectType.Car ||
                            g.GameObjectType == GameObjectType.Roadblock ||
                            g.GameObjectType == GameObjectType.Pedestrian ||
                            g.GameObjectType == GameObjectType.BusStop)
                .ToList();

            //var nearbyLanes = _lanes
            //    .Where(lane => Math.Abs(_currentLaneId - lane.Id) <= 1);
            var laneScores = _lanes
                .Select(lane => new
                {
                    Key = lane.Id,
                    Value = lane.GetScore(allObstacleObjects, self, _currentLaneId)
                })
                .ToList();
            var bestLaneScore = laneScores.Min(l => l.Value);
            var bestLaneId = laneScores.First(l => l.Value == bestLaneScore).Key;
            Debug.Write($"{bestLaneId} ");
            Debug.Write($"{_lanes[0].GetScore(allObstacleObjects, self, _currentLaneId)} ");
            Debug.Write($"{_lanes[1].GetScore(allObstacleObjects, self, _currentLaneId)} ");
            Debug.Write($"{_lanes[2].GetScore(allObstacleObjects, self, _currentLaneId)} ");
            Debug.Write($"{_lanes[3].GetScore(allObstacleObjects, self, _currentLaneId)} ");
            Debug.WriteLine("");
            if (!_isChangingLanes && bestLaneId != _currentLaneId)
            {
                _isChangingLanes = true;
                _currentLaneId = bestLaneId;
            }
            var desiredX = (_currentLaneId * 3) - _laneOffset + GameConstants.LaneWidth / 2;

            //var otherInFront = _lanes[_currentLaneId].GetClosestObjectInFront(allObstacleObjects, self);
            //if (otherInFront != null)
            //{
            //    var otherSpeed = obstacleInFront.VY;
            //    var otherDistanceToStop = CalculateDistanceToStop(otherSpeed, obstacleInFront.MaximumDeceleration);
            //    var selfDistanceToStop = CalculateDistanceToStop(self.VY, self.MaximumDeceleration);

            //    var selfCenterY = (self.BoundingBox.Top + self.BoundingBox.Bottom) / 2;
            //    var otherCenterY = (obstacleInFront.BoundingBox.Top + obstacleInFront.BoundingBox.Bottom) / 2;

            //    var distanceBetweenCars = Math.Abs(selfCenterY - otherCenterY) - self.BoundingBox.Height / 2 -
            //                              obstacleInFront.BoundingBox.Height / 2;
            //    const float plusSafeDistance = 1;
            //foreach (var lane in _lanePriority)
            //{
            //    var dist = obstaclesInFront[lane].GetDistance(self);
            //    if (dist > 50)
            //    {
            //        _currentLaneId = lane;
            //        break;
            //    }
            //}
            //if (otherInFront.GetDistance(self) < self.DistanceToStop - otherInFront.DistanceToStop + plusSafeDistance &&
            //    self.VY > 0)
            //{
            //    accelerationY = -1;
            //}
            //}

            var centerX = (self.BoundingBox.Left + self.BoundingBox.Right) / 2;
            if (Math.Abs(desiredX - centerX) > 0.2)
            {
                if (desiredX < centerX)
                    left = true;
                else
                    right = true;
            }
            else
            {
                _isChangingLanes = false;
            }

            return new PlayerAction()
            {
                MoveLeft = left,
                MoveRight = right,
                Acceleration = accelerationY
            };
        }

        //private IEnumerable<GameObjectExtended> GetGameObjectsInFront(IReadOnlyCollection<GameObjectState> objects,
        //    GameObjectState self)
        //{
        //    var bound = self.BoundingBox;
        //    var lanes = new RectangleF[_laneCount]
        //    {
        //        new RectangleF(-4.5f, bound.Y, bound.Width, bound.Height),
        //        new RectangleF(-1.5f, bound.Y, bound.Width, bound.Height),
        //        new RectangleF(1.5f, bound.Y, bound.Width, bound.Height),
        //        new RectangleF(4.5f, bound.Y, bound.Width, bound.Height),
        //    };

        //    var gameObjectsInFront = lanes
        //        .Select(lane => GetClosestObjectInFront(objects, lane));

        //    return gameObjectsInFront
        //        .Select(o => new GameObjectExtended(o));
        //}

        //private GameObjectState GetClosestObjectInFront(IEnumerable<GameObjectState> objects, GameObjectState self)
        //{
        //    return GetClosestObjectInFront(objects, self.BoundingBox);
        //}

        //private static GameObjectState GetClosestObjectInFront(IEnumerable<GameObjectState> objects,
        //    RectangleF selfBoundingBox)
        //{
        //    return objects
        //        .Where(o => o.BoundingBox.Y > selfBoundingBox.Y)
        //        .FirstOrDefault(o => IsOverlappingHorizontally(selfBoundingBox, o.BoundingBox));
        //}

        //private bool IsOverlappingHorizontally(GameObjectState self, GameObjectState other)
        //{
        //    var r1 = self.BoundingBox;
        //    var r2 = other.BoundingBox;
        //    return Between(r1.Left, r1.Right, r2.Left) ||
        //           Between(r1.Left, r1.Right, r2.Right) ||
        //           Between(r2.Left, r2.Right, r1.Right) ||
        //           Between(r2.Left, r2.Right, r1.Left);
        //}

        //private static bool IsOverlappingHorizontally(RectangleF r1, RectangleF r2)
        //{
        //    return Between(r1.Left, r1.Right, r2.Left) ||
        //           Between(r1.Left, r1.Right, r2.Right) ||
        //           Between(r2.Left, r2.Right, r1.Right) ||
        //           Between(r2.Left, r2.Right, r1.Left);
        //}

        //private static bool Between(float limit1, float limit2, float value)
        //{
        //    return (value >= limit1 && value <= limit2) || (value >= limit2 && value <= limit1);
        //}
    }
}