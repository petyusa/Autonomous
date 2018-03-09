using System.ComponentModel.Composition;
using Microsoft.Xna.Framework.Input;
using Autonomous.Public;

namespace Autonomous.HumanPlayer
{
    //[Export(typeof(IPlayer))]
    [ExportMetadata("PlayerName", "Human")]
    public class HumanPlayer : IPlayer
    {
        public void Finish()
        {
        }

        public void Initialize(string playerId)
        {
        }

        public PlayerAction Update(GameState gameState)
        {
            float accelerationY = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                accelerationY = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                accelerationY = -1;

            bool left = Keyboard.GetState().IsKeyDown(Keys.Left);
            bool right = Keyboard.GetState().IsKeyDown(Keys.Right);

            return new PlayerAction() { MoveLeft = left, MoveRight = right, Acceleration = accelerationY };
        }
    }
}
