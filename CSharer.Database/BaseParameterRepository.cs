using MySql.Data.MySqlClient;

namespace CSharer.Database
{
    public class BaseParameterRepository
    {
        private readonly ConnectionSql _connectionSql = new();
        private readonly string _tableName;

        public BaseParameterRepository(string tableName)
        {
            _tableName = tableName;
        }

        // CREATE
        public void Add(string content)
        {
            using var conn = _connectionSql.GetConnection();
            conn.Open();

            string query = $"INSERT INTO {_tableName} (Content, IsActive) VALUES (@Content, 1)";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Content", content);

            cmd.ExecuteNonQuery();
        }

        // READ ALL
        public List<(int Id, string Content)> GetAll()
        {
            var list = new List<(int, string)>();

            using var conn = _connectionSql.GetConnection();
            conn.Open();

            string query = $"SELECT Id, Content FROM {_tableName} WHERE IsActive = 1";

            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add((
                    Convert.ToInt32(reader["Id"]),
                    reader["Content"].ToString() ?? ""
                ));
            }

            return list;
        }

        // UPDATE
        public void Update(int id, string newContent)
        {
            using var conn = _connectionSql.GetConnection();
            conn.Open();

            string query = $"UPDATE {_tableName} SET Content = @Content WHERE Id = @Id";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Content", newContent);

            cmd.ExecuteNonQuery();
        }

        // DELETE 
        public void Delete(int id)
        {
            using var conn = _connectionSql.GetConnection();
            conn.Open();

            string query = $"UPDATE {_tableName} SET IsActive = 0 WHERE Id = @Id";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        // FOR AI 
        public List<string> GetAllActiveContents()
        {
            return GetAll().Select(x => x.Content).ToList();
        }
    }
}