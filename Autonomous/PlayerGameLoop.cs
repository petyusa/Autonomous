using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Autonomous.Public;

namespace Autonomous.Impl
{
    class PlayerGameLoop
    {
        public static void StartGameLoop(IPlayer player, string playerId, GameStateManager gameStateManager)
        {
            try
            {
                player.Initialize(playerId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Thread t = new Thread(() => 
            
            {
                bool stopped = false;
                int lastState = -1;
                while (!stopped)
                {
                    stopped = gameStateManager.GameState?.Stopped ?? false;
                    if (gameStateManager.GameStateCounter == lastState)
                    {
                        Thread.Sleep(50);
                        continue;
                    }

                    lastState = gameStateManager.GameStateCounter;

                    var state = gameStateManager.GameState;
                    if (state == null)
                        continue;

                    try
                    {
                        var command = player.Update(state);
                        gameStateManager.SetPlayerCommand(playerId, command);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                player.Finish();
            });
            t.IsBackground = true;
            t.Start();
        }
    }
}
