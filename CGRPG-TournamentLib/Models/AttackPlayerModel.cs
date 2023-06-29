using System;

namespace CGRPG_TournamentLib.Models
{
    public class AttackPlayerModel
    {
        public Guid BattleId { get; set; }
        public string MetaMaskAddress { get; set; } = null!;
        public string MetaMaskAddressOpponent { get; set; } = null!;
        public bool DidIWin { get; set; }
    }
}