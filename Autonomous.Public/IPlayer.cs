using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomous.Public
{
    /// <summary>
    /// Interface which should be implemented by the players.
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// Called when the game starts. <paramref name="playerId"/> will be used
        /// to identify <see cref="GameObjectState"/> belongs to the player.
        /// </summary>
        /// <param name="playerId"></param>
        void Initialize(string playerId);
        /// <summary>
        /// Called in each loop. Player can respond with the action to perform.
        /// </summary>
        /// <param name="gameState"></param>
        /// <returns></returns>
        PlayerAction Update(GameState gameState);
        /// <summary>
        /// Called when the game is finished.
        /// </summary>
        void Finish();
    }

}
