using Npgsql;

namespace Infrastructure.Data
{
    public class ApplicationDbContext
    {
        private readonly string _connString = "Host=localhost;Port=5432;Database=exam170126;Username=postgres;Password=2345";

        public NpgsqlConnection Connection() => new NpgsqlConnection(_connString);
    }
}
