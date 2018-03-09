
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using Autonomous.Public;

namespace Autonomous.ReallyCleverPlayer
{
    [Export(typeof(IPlayer))]
    [ExportMetadata("PlayerName", "NotSoCleverPlayer")]
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public class ReallyCleverPlayer : IPlayer
    {
        private string _playerId;
        public void Initialize(string playerId)
        {
            _playerId = playerId;
        }

        public PlayerAction Update(GameState gameState)
        {
            throw new System.NotImplementedException();
        }

        public void Finish()
        {
        }
    }
}
