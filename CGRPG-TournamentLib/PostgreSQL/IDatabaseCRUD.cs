using Npgsql;

namespace CGRPG_TournamentLib.PostgreSQL
{
    public abstract class DatabaseCrud<T>
    {
        public abstract NpgsqlBatchCommand Add(T poco);
        public abstract NpgsqlBatchCommand Update(T poco);
        public abstract T Read(NpgsqlDataReader reader);
        public abstract NpgsqlBatchCommand Delete(T poco);
        public abstract string GetTableName();
    }
}