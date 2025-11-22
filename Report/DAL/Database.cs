using Npgsql;

namespace ReportUI.DAL
{
    public class Database
    {
        private static string connString =
            "Host=localhost;Port=5433;Username=postgres;Password=123456;Database=postgres";

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connString);
        }
    }
}
