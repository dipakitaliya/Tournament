using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CGRPG_Tournament.PostgreSQL;
using CGRPG_TournamentLib.Models;
using CommandLine;
using Npgsql;

namespace CGRPG_TournamentLib.Contexts
{
    public class Options
    {
        [Option("pgaddr", Default = (string)"127.0.0.1")]
        public string Address { get; set; } = null!;

        [Option("pgport", Default = (int)5432)]
        public int Port { get; set; }

        [Option("pgdb", Default = (string)"postgres")]
        public string Database { get; set; } = null!;

        [Option("pguser", Default = (string)"citrus")]
        public string Username { get; set; } = null!;

        [Option("pgpass", Default = (string)"trityiputty")]
        public string Password { get; set; } = null!;
    }

    public class TournamentContext
    {
        private readonly NpgsqlConnection _connection;

        public NpgsqlConnection Connection => _connection;

        public static Options? Options { get; set; }
        
        public DatabaseSet<UserModel> Users { get; set; }
        
        public DatabaseSet<BattleModel> Battles { get; set; }

        public TournamentContext()
        {
            string connectionString =
                "Host=127.0.0.1;Port=5432;Database=postgres;Username=citrus;Password=trityiputty;";
            if (Options != null)
            {
                connectionString = $"Host={Options.Address};" +
                                   $"Port={Options.Port.ToString()};" +
                                   $"Database={Options.Database};" +
                                   $"Username={Options.Username};" +
                                   $"Password={Options.Password};";
            }

            _connection = new NpgsqlConnection(connectionString);
            
            Users = new DatabaseSet<UserModel>(_connection);
            Battles = new DatabaseSet<BattleModel>(_connection);
        }
        
        public async Task SaveChangesAsync()
        {
            await _connection.OpenAsync();
            using (var batch = new NpgsqlBatch(_connection))
            {
                var user = Users.GetStatements();
                var battle = Battles.GetStatements();
                foreach (var statement in user)
                {
                    batch.BatchCommands.Add(statement);
                }
                foreach (var statement in battle)
                {
                    batch.BatchCommands.Add(statement);
                }

                await using var reader = await batch.ExecuteReaderAsync();
                user.Clear();
                battle.Clear();
            }
            await _connection.CloseAsync();
        }

        public async Task<bool> CheckIfRecordExists(string table, string column, string value)
        {
            await _connection.OpenAsync();
            using var cmd = new NpgsqlCommand($"select exists(select 1 from {table} where {column}={value})", _connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                bool? result = reader["exists"] as bool?;
                _connection.Close();
                Debug.Assert(result != null, nameof(result) + " != null");
                return result!.Value;
            }
            await _connection.CloseAsync();
            return true;
        }

        public async Task<int> RemoveAll(string table)
        {
            await _connection.OpenAsync();
            using var cmd = new NpgsqlCommand($"delete from {table}", _connection);
            var reader = await cmd.ExecuteNonQueryAsync();
            Console.Write($"From {table} deleted {reader.ToString()} rows.");
            await _connection.CloseAsync();
            return reader;
        }
    }
}