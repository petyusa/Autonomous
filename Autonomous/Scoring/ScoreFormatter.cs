namespace Autonomous.Impl.Scoring
{
    internal class ScoreFormatter
    {
        public string GetFormattedScore(PlayerScore score)
        {
            if (score == null) return null;

            return score.Stopped
                ? $"{score.Position}. {score.PlayerName}: STOPPED"
                : $"{score.Position}. {score.PlayerName}:" +
                   $" {score.Distance}m " +
                   $" Speed: {score.Speed}km/h" +
                   $" Damage: {score.DamageInPercent}%";
        }
    }
}
