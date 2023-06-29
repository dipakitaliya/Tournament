using System;
using Npgsql;

namespace CGRPG_TournamentLib.Models
{
    public class SettingsModel
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string TournamentName { get; set; } = null!;
        
        public int BattleTicketPrice { get; set; }
        
        public int BattleTicketCount{ get; set; }
        
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DateTime TournamentStartDate { get; set; }
        
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public DateTime TournamentEndDate { get; set; }
        
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int TotalUsersJoined { get; set; }

        public void Parse(NpgsqlDataReader reader)
        {
            TournamentName = reader["tournament_name"] as string ?? "Chain Gauntlet";
            BattleTicketPrice = reader["battle_ticket_price"] as int? ?? 1000;
            BattleTicketCount = reader["battle_ticket_count"] as int? ?? 5;
            TournamentStartDate = ((reader["tournament_start"] as DateTime?)!).Value;
            TournamentEndDate = ((reader["tournament_end"] as DateTime?)!).Value;
        }
    }
}