using System.Collections.Generic;

namespace Autonomous.Public
{
    /// <summary>
    /// State of the game. 
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// [INtERNAL] Constructor.
        /// </summary>
        /// <param name="gameObjectStates"></param>
        /// <param name="stopped"></param>
        public GameState(IReadOnlyList<GameObjectState> gameObjectStates, bool stopped, bool playerCollision)
        {
            GameObjectStates = gameObjectStates;
            Stopped = stopped;
            PlayerCollision = playerCollision;
        }

        /// <summary>
        /// State of all objects the player interacts with, ordered by bounding box center ascending.
        /// </summary>
        public IReadOnlyList<GameObjectState> GameObjectStates { get; private set; }

        /// <summary>
        /// True if the game is finished, otherwise false.
        /// </summary>
        public bool Stopped { get; private set; }

        /// <summary>
        /// True if the collision is detected between players.
        /// </summary>
        public bool PlayerCollision { get; private set; }
    }
}