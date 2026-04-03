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

    public class DefaultSettings
    {
        public string Hashtags { get; set; } = string.Empty;
    }
}
