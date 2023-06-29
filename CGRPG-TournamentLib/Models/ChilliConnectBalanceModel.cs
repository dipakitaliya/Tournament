using System;
using System.Collections.Generic;

namespace CGRPG_TournamentLib.Models
{

    public class Balances
    {
        public string Key;
        public string Name;
        public long Balance;
    }

    public class ChilliConnectBalanceModel
    {
        public Guid CatalogVersion { get; set; }

        public List<Balances> Balances { get; set; }
    }
}