using MySql.Data.MySqlClient;
using CSharer.Core.Models;

namespace CSharer.Database
{
    public class VideoPostRepository
    {
        private readonly ConnectionSql _connectionSql;

        public VideoPostRepository()
        {
            _connectionSql = new ConnectionSql();
        }

        // CREATE
        public void AddVideoPost(VideoPost videoPost)
        {
            using var conn = _connectionSql.GetConnection();
            conn.Open();

            string query = @"INSERT INTO videoposts 
                            (VideoPath, FileName, GeneratedCaption, DetectedAt, PostedToBuffer, PostedToPinterest)
                            VALUES
                            (@VideoPath, @FileName, @GeneratedCaption, @DetectedAt, @PostedToBuffer, @PostedToPinterest)";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@VideoPath", videoPost.VideoPath);
            cmd.Parameters.AddWithValue("@FileName", videoPost.FileName);
            cmd.Parameters.AddWithValue("@GeneratedCaption", videoPost.GeneratedCaption);
            cmd.Parameters.AddWithValue("@DetectedAt", videoPost.DetectedAt);
            cmd.Parameters.AddWithValue("@PostedToBuffer", videoPost.PostedToBuffer);
            cmd.Parameters.AddWithValue("@PostedToPinterest", videoPost.PostedToPinterest);

            cmd.ExecuteNonQuery();
        }

        // READ ALL
        public List<VideoPost> GetAllVideoPosts()
        {
            var videoPosts = new List<VideoPost>();

            using var conn = _connectionSql.GetConnection();
            conn.Open();

            string query = "SELECT * FROM videoposts";

            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                videoPosts.Add(new VideoPost
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    VideoPath = reader["VideoPath"].ToString() ?? "",
                    FileName = reader["FileName"].ToString() ?? "",
                    GeneratedCaption = reader["GeneratedCaption"].ToString() ?? "",
                    DetectedAt = Convert.ToDateTime(reader["DetectedAt"]),
                    PostedToBuffer = Convert.ToBoolean(reader["PostedToBuffer"]),
                    PostedToPinterest = Convert.ToBoolean(reader["PostedToPinterest"])
                });
            }

            return videoPosts;
        }

        // READ ONE
        public VideoPost? GetVideoPostById(int id)
        {
            using var conn = _connectionSql.GetConnection();
            conn.Open();

            string query = "SELECT * FROM videoposts WHERE Id = @Id";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new VideoPost
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    VideoPath = reader["VideoPath"].ToString() ?? "",
                    FileName = reader["FileName"].ToString() ?? "",
                    GeneratedCaption = reader["GeneratedCaption"].ToString() ?? "",
                    DetectedAt = Convert.ToDateTime(reader["DetectedAt"]),
                    PostedToBuffer = Convert.ToBoolean(reader["PostedToBuffer"]),
                    PostedToPinterest = Convert.ToBoolean(reader["PostedToPinterest"])
                };
            }

            return null;
        }

        // UPDATE
        public void UpdateVideoPost(VideoPost videoPost)
        {
            using var conn = _connectionSql.GetConnection();
            conn.Open();

            string query = @"UPDATE videoposts
                             SET VideoPath = @VideoPath,
                                 FileName = @FileName,
                                 GeneratedCaption = @GeneratedCaption,
                                 DetectedAt = @DetectedAt,
                                 PostedToBuffer = @PostedToBuffer,
                                 PostedToPinterest = @PostedToPinterest
                             WHERE Id = @Id";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", videoPost.Id);
            cmd.Parameters.AddWithValue("@VideoPath", videoPost.VideoPath);
            cmd.Parameters.AddWithValue("@FileName", videoPost.FileName);
            cmd.Parameters.AddWithValue("@GeneratedCaption", videoPost.GeneratedCaption);
            cmd.Parameters.AddWithValue("@DetectedAt", videoPost.DetectedAt);
            cmd.Parameters.AddWithValue("@PostedToBuffer", videoPost.PostedToBuffer);
            cmd.Parameters.AddWithValue("@PostedToPinterest", videoPost.PostedToPinterest);

            cmd.ExecuteNonQuery();
        }

        // DELETE
        public void DeleteVideoPost(int id)
        {
            using var conn = _connectionSql.GetConnection();
            conn.Open();

            string query = "DELETE FROM videoposts WHERE Id = @Id";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }
    }
}