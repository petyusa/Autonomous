using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autonomous.Impl.GameObjects;

namespace Autonomous.Impl
{
    public class GameStateInternal
    {
        public bool Stopped { get; set; }
        public bool PlayerCollision { get; set; }

        private IList<GameObject> _gameObjects;

        public IList<GameObject> GameObjects
        {
            get => _gameObjects;
            set
            {
                _gameObjects = value;
                for (int i = 0; i < _gameObjects.Count; i++)
                {
                    if (!(_gameObjects[i] is Car))
                        continue;
                    if (LastPlayer == null)
                    {
                        LastPlayer = _gameObjects[i] as Car;
                        FirstPlayerIndex = i;
                    }
                    FirstPlayer = _gameObjects[i] as Car;
                    FirstPlayerIndex = i;
                }
            }
        }


        public Car FirstPlayer
        {
            get;
            private set;
        }

        public int FirstPlayerIndex { get; private set; }

        public Car LastPlayer
        {
            get;
            private set;
        }
    }
    public interface IGameStateProvider
    {
        GameStateInternal GameStateInternal { get; }
    }
}
