using System.Collections.Generic;
using System.Threading.Tasks;
using CGRPG_TournamentLib.PostgreSQL;
using Npgsql;

namespace CGRPG_Tournament.PostgreSQL
{
    public class DatabaseSet<T> where T : DatabaseCrud<T>, new()
    {
        private T obj = new T();

        private List<NpgsqlBatchCommand> Statements = new List<NpgsqlBatchCommand>();

        private NpgsqlConnection connection;

        public DatabaseSet(NpgsqlConnection conn)
        {
            connection = conn;
        }

        public List<NpgsqlBatchCommand> GetStatements()
        {
            return Statements;
        }

        public void Add(T poco)
        {
            Statements.Add(obj.Add(poco));
        }
        
        public void Update(T poco)
        {
            Statements.Add(obj.Update(poco));
        }

        public async Task<T?> SingleOrDefault(string column, string value)
        {
            await connection.OpenAsync();
            using (var cmd = new NpgsqlCommand($"select * from {obj.GetTableName()} where {column} = {value}", connection))
            {
                using (var reader = await cmd.ExecuteReaderAsync()) 
                {
                    while (await reader.ReadAsync())
                    {
                        T result = obj.Read(reader);
                        await connection.CloseAsync();
                        return result;
                    }
                }
            }
            await connection.CloseAsync();
            return new T();
        }
        
        public async Task<T?> Single(string column, string value)
        {
            await connection.OpenAsync();
            using (var cmd = new NpgsqlCommand($"select * from {obj.GetTableName()} where {column} = {value}", connection))
            {
                using (var reader = await cmd.ExecuteReaderAsync()) 
                {
                    while (await reader.ReadAsync())
                    {
                        T? result = obj.Read(reader);
                        await connection.CloseAsync();
                        return result;
                    }
                }
            }
            await connection.CloseAsync();
            return null;
        }

        public async Task<List<T>> ToListAsync()
        {
            List<T> list = new List<T>();
            connection.Open();
            using (var cmd = new NpgsqlCommand($"select * from {obj.GetTableName()}", connection))
            {
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        T result = obj.Read(reader);
                        list.Add(result);
                    }
                }
            }
            connection.Close();
            return list;
        }
    }
}