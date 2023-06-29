using System;
using System.Globalization;
using CGRPG_Tournament.PostgreSQL;
using CGRPG_TournamentLib.PostgreSQL;
using Newtonsoft.Json;
using Npgsql;

namespace CGRPG_TournamentLib.Models
{
    public class BattleModel : DatabaseCrud<BattleModel>
    {
        [JsonIgnore] 
        private const string TableName = "battles";

        public Guid BattleId { get; set; }
        
        public string MetaMaskAddress { get; set; } = null!;

        public string MetaMaskAddressOpponent { get; set; } = null!;

        public double BattlePower { get; set; }
        
        public double BattlePowerOpponent { get; set; }

        public DateTime OnCreated { get; set; } = DateTime.Now;
        
        public bool IsBattleConcluded { get; set; }
        
        public bool IsBattleFlagged { get; set; }

        public override NpgsqlBatchCommand Add(BattleModel poco)
        {
            string commandText = 
                $"INSERT INTO {TableName} " +
                $"(battle_id," +
                $"metamask_address," +
                $"metamask_address_opponent," +
                $"battlepower," +
                $"battlepower_opponent," +
                $"on_created," +
                $"concluded," +
                $"flagged) " +
                $"VALUES (" +
                $"'{poco.BattleId.ToString()}'," +
                $"'{poco.MetaMaskAddress}'," +
                $"'{poco.MetaMaskAddressOpponent}'," +
                $"{poco.BattlePower.ToString(CultureInfo.InvariantCulture)}," +
                $"{poco.BattlePowerOpponent.ToString(CultureInfo.InvariantCulture)}," +
                $"'{poco.OnCreated.ToString("yyyy-MM-ddTHH:mm:ssK")}'," +
                $"{poco.IsBattleConcluded.ToString()}," +
                $"{poco.IsBattleFlagged.ToString()})";
            return new NpgsqlBatchCommand(commandText);
        }

        public override NpgsqlBatchCommand Update(BattleModel poco)
        {
            var commandText = $@"UPDATE {TableName} SET 
                           metamask_address = '{poco.MetaMaskAddress}',
                           metamask_address_opponent = '{poco.MetaMaskAddressOpponent}',
                           battlepower = {poco.BattlePower.ToString(CultureInfo.InvariantCulture)},
                           battlepower_opponent = {poco.BattlePowerOpponent.ToString(CultureInfo.InvariantCulture)},
                           on_created = '{poco.OnCreated.ToString("yyyy-MM-ddTHH:mm:ssK")}',
                           concluded = {poco.IsBattleConcluded.ToString()},
                           flagged = {poco.IsBattleFlagged.ToString()}
                WHERE battle_id = '{poco.BattleId.ToString()}'";
            return new NpgsqlBatchCommand(commandText);
        }

        public override BattleModel Read(NpgsqlDataReader reader)
        {
            Guid? battleId = reader["battle_id"] as Guid?;
            string? metamaskAddress = reader["metamask_address"] as string;
            string? metamaskAddressOpponent = reader["metamask_address_opponent"] as string;
            int? battlepower = reader["battlepower"] as int?;
            int? battlepowerOpponent = reader["battlepower_opponent"] as int?;
            DateTime? onCreated = reader["on_created"] as DateTime?;
            bool? concluded = reader["concluded"] as bool?;
            bool? flagged = reader["flagged"] as bool?;

            var battle = new BattleModel()
            {
                BattleId = battleId!.Value,
                MetaMaskAddress = metamaskAddress!,
                MetaMaskAddressOpponent = metamaskAddressOpponent!,
                BattlePower = battlepower!.Value,
                BattlePowerOpponent = battlepowerOpponent!.Value,
                OnCreated = onCreated!.Value,
                IsBattleConcluded = concluded!.Value,
                IsBattleFlagged = flagged!.Value,
            };
            return battle;
        }

        public override NpgsqlBatchCommand Delete(BattleModel poco)
        {
            throw new NotImplementedException();
        }

        public override string GetTableName()
        {
            return TableName;
        }

        

    }
}