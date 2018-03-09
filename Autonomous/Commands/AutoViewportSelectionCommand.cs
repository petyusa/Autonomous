using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using Autonomous.Impl.GameObjects;
using Autonomous.Impl.Viewports;
using Microsoft.Xna.Framework.Input;

namespace Autonomous.Impl.Commands
{
    class AutoViewportSelectionCommand : IGameCommand
    {
        private readonly ViewportManager viewportManager;
        private readonly List<Car> players;
        private readonly int humanPlayerIndex;
        private readonly int changeInterval;
        private TimeSpan lastViewportChange = new TimeSpan();
        private int activePlayerIndex;
        private int activeCameraIndex;
        private bool _enabled;

        public AutoViewportSelectionCommand(ViewportManager viewportManager, List<Car> players, int humanPlayerIndex, int changeInterval = 10000)
        {
            this.viewportManager = viewportManager;
            this.players = players;
            this.humanPlayerIndex = humanPlayerIndex;
            this.changeInterval = changeInterval;
        }

        public void Handle(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F9))
            {
                ToogleEnabled();
            }

            if (!_enabled) return;

            if ((gameTime.TotalGameTime - lastViewportChange).TotalMilliseconds > changeInterval)
            {
                var playersInFocus = new List<Car>();
                if (activePlayerIndex == players.Count)
                {
                    playersInFocus = players;
                }
                else
                {
                    if (humanPlayerIndex != -1)
                        playersInFocus.Add(players[humanPlayerIndex]);

                    if (activePlayerIndex != humanPlayerIndex)
                        playersInFocus.Add(players[activePlayerIndex]);
                }

                viewportManager.SetViewports(playersInFocus, GetActiveCamera());

                activePlayerIndex = (++activePlayerIndex) % (players.Count + 1);

                lastViewportChange = gameTime.TotalGameTime;
            }
        }

        private void ToogleEnabled()
        {
            _enabled = !_enabled;
        }

        public CameraSetup GetActiveCamera()
        {
            var setup = CameraSetup.CameraDefault;

            if (activeCameraIndex % 3 == 0) setup = CameraSetup.CameraInside;

            if (activeCameraIndex % 4 == 0) setup = CameraSetup.CameraRear;

            ++activeCameraIndex;
            return setup;
        }
    }
}
