using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace CSharer.Database
{
    public class ConnectionSql
    {
        private readonly string _connectionString;

        public ConnectionSql()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Connection string not found in appsettings.json");
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}