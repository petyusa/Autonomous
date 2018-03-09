using Microsoft.Xna.Framework;

namespace Autonomous.Impl.Commands
{
    interface IGameCommand
    {
        void Handle(GameTime gameTime);
    }
}
