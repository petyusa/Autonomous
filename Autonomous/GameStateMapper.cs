using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autonomous.Public;
using Autonomous.Impl.GameObjects;

namespace Autonomous.Impl
{
    class GameStateMapper
    {
        public static GameState GameStateToPublic(GameStateInternal state)
        {
            return new GameState(state.GameObjects.Select(GameObjectStateToPublic).ToList(), state.Stopped, state.PlayerCollision);
        }
        private static GameObjectState GameObjectStateToPublic(GameObject gameObject)
        {
            Car car = gameObject as Car;
            return new GameObjectState(gameObject.Id, gameObject.Type, gameObject.BoundingBox, gameObject.VX,
               gameObject.OppositeDirection ? -gameObject.VY : gameObject.VY, car?.Damage ?? 0);
        }
    }
}
