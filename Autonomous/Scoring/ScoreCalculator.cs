using System;
using System.Collections.Generic;
using System.Linq;
using Autonomous.Impl.GameObjects;

namespace Autonomous.Impl.Scoring
{
    internal class ScoreCalculator
    {
        public IEnumerable<PlayerScore> GetPlayerScores(IEnumerable<Car> players, TimeSpan timeElapsed)
        {
            var sortedPlayers = players.OrderByDescending(car => car.Y);

            int position = 1;
            foreach (var player in sortedPlayers)
            {
                var distance = (int)Math.Round(player.Y);
                var damageInPercent = (int)Math.Round(player.Damage * 100);
                var speed = (int)Math.Round(player.VY * 4);
                var score = player.Stopped || position > 3 ? 0 : (int)Math.Pow(2, 4 - position);

                yield return new PlayerScore(player.PlayerName,
                    distance, position, damageInPercent, speed, timeElapsed, player.Stopped, score);

                ++position;
            }
        }
    }
}
