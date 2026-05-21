using MySql.Data.MySqlClient;

namespace CSharer.Database
{
    public class ConnectionSql
    {
        private readonly string _connectionString =
            "server=localhost;database=csharer_db;uid=root;pwd=Carbos123*;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}