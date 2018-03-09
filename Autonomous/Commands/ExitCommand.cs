using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Autonomous.Impl.Commands
{
    class ExitCommand : IGameCommand
    {
        private readonly Action exit;

        public ExitCommand(Action exit)
        {
            this.exit = exit;
        }

        public void Handle(GameTime gameTime)
        {
            try
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    exit();
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
