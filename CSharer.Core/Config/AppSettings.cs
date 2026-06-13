namespace CSharer.Core.Config
{
    public class AppSettings
    {
        public string WatchFolder { get; set; } = string.Empty;
        public ApiSettings APIs { get; set; } = new();
        public DefaultSettings Defaults { get; set; } = new();
    }

    public class ApiSettings
    {
        public GroqSettings Groq { get; set; } = new();
        public YouTubeSettings YouTube { get; set; } = new();
        public BufferSettings Buffer { get; set; } = new();
        public CloudinarySettings Cloudinary { get; set; } = new();
    }

    public class GroqSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "llama-3.3-70b-versatile";
    }

    public class YouTubeSettings
    {
        public string ClientSecretPath { get; set; } = string.Empty;
    }

    public class BufferSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public List<BufferChannel> Channels { get; set; } = new();
    }

    public class BufferChannel
    {
        public string Id { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
    }

    public class CloudinarySettings
    {
        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
    }

    public class DefaultSettings
    {
        public string Hashtags { get; set; } = string.Empty;
    }
}