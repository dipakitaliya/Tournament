using System.Collections.Generic;

namespace CGRPG_TournamentLib.Models
{
    public class ChilliConnectBalanceResponseModel
    {
        // ReSharper disable once UnusedMember.Global
        public string CatalogVersion { get; set; } = null!;
        public List<ChilliConnectBalanceModel> Balances { get; set; } = null!;
    }
}