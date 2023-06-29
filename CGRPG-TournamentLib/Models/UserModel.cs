using System;
using System.Globalization;
using CGRPG_Tournament.PostgreSQL;
using CGRPG_TournamentLib.PostgreSQL;
using Newtonsoft.Json;
using Npgsql;

namespace CGRPG_TournamentLib.Models
{ 
    public class UserModel : DatabaseCrud<UserModel>
    {
        [JsonIgnore]
        public readonly string TableName = "usermodel";

        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public int BattleTickets { get; set; }

        public double EloRating { get; set; }
        
        public string MetaMaskAddress { get; set; } = null!;

        public string? Units { get; set; } = null!;

        public int BattlePower { get; set; }
        
        public int Wins { get; set; }
        
        public int Losses { get; set; }
        
        public int TotalBattles { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.Now;
        
        public override NpgsqlBatchCommand Add(UserModel poco)
        {
            string commandText = 
                $"INSERT INTO {TableName} (" +
                $"tickets," +
                $"username," +
                $"elo_rating," +
                $"mm_address," +
                $"units," +
                $"battlepower," +
                $"wins," +
                $"losses," +
                $"totalBattles," +
                $"on_joined) " +
                $"VALUES (" +
                $"{poco.BattleTickets.ToString()}," +
                $"'{poco.Username}'," +
                $"{poco.EloRating.ToString(CultureInfo.InvariantCulture)}," +
                $"'{poco.MetaMaskAddress}'," +
                $"'{poco.Units}'," +
                $"{poco.BattlePower.ToString()}," +
                $"{poco.Wins.ToString()}," +
                $"{poco.Losses.ToString()}," +
                $"{poco.TotalBattles.ToString()}," +
                $"'{poco.Timestamp.ToString("yyyy-MM-ddTHH:mm:ssK")}')";
            return new NpgsqlBatchCommand(commandText);
        }

        public override NpgsqlBatchCommand Update(UserModel poco)
        {
            var commandText = $@"UPDATE {TableName} SET 
                           elo_rating = {poco.EloRating.ToString(CultureInfo.InvariantCulture)},
                           username = '{poco.Username}',
                           on_joined = '{poco.Timestamp.ToString("yyyy-MM-ddTHH:mm:ssK")}',
                           mm_address = '{poco.MetaMaskAddress}',
                           units = '{poco.Units}',
                           battlepower = {poco.BattlePower.ToString()},
                           wins = {poco.Wins.ToString()},
                           losses = {poco.Losses.ToString()},
                           totalBattles = {poco.TotalBattles.ToString()},
                           tickets = {poco.BattleTickets.ToString()}
                           WHERE user_id = {poco.Id.ToString()}";
            return new NpgsqlBatchCommand(commandText);
        }

        public override UserModel Read(NpgsqlDataReader reader)
        {
            int? id = reader["user_id"] as int?;
            string? username = reader["username"] as string;
            double? eloRating = reader["elo_rating"] as double?;
            int? tickets = reader["tickets"] as int?;
            DateTime? onJoined = reader["on_joined"] as DateTime?;
            string? mmAddress = reader["mm_address"] as string;
            string? units = reader["units"] as string;
            int? battlepower = reader["battlepower"] as int?;
            int? wins = reader["wins"] as int?;
            int? losses = reader["losses"] as int?;
            int? totalBattles = reader["totalBattles"] as int?;

            var user = new UserModel
            {
                Id = id!.Value,
                Username = username!,
                EloRating = eloRating!.Value,
                BattleTickets = tickets!.Value,
                Timestamp = onJoined!.Value,
                MetaMaskAddress = mmAddress!,
                Units = units,
                BattlePower = battlepower!.Value,
                Wins = wins!.Value,
                Losses = losses!.Value,
                TotalBattles = totalBattles!.Value
            };
            return user;
        }

        public override NpgsqlBatchCommand Delete(UserModel poco)
        {
            throw new NotImplementedException();
        }

        public override string GetTableName()
        {
            return TableName;
        }
    }
}