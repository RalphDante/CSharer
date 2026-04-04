using Microsoft.Extensions.Configuration;
using CSharer.Core.Config;
using CSharer.Core.Services;

namespace CSharer.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.WriteLine("CSharer - YouTube Auto-Uploader\n");

            var basePath = AppContext.BaseDirectory;
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var settings = config.Get<AppSettings>() ?? throw new Exception("Failed to load settings");

            // Initialize services
            var aiService = new AIService(settings.APIs.Groq.ApiKey, settings.APIs.Groq.Model);
            var youtubeService = new YouTubeUploadService(settings.APIs.YouTube.ClientSecretPath);
            var fileWatcher = new FileWatcherService();

            fileWatcher.OnNewVideoDetected += async (videoPath) =>
            {
                try
                {
                    System.Console.WriteLine($"\nGenerating metadata...");

                    var title = await aiService.GenerateStudyTitle();
                    var description = await aiService.GenerateStudyDescription(title);
                    var hashtags = await aiService.GenerateHashtags();

                    System.Console.WriteLine($"Title: {title}");
                    System.Console.WriteLine($"Description: {description}");
                    System.Console.WriteLine($"Hashtags: {hashtags}");

                    System.Console.WriteLine("\nUploading to YouTube...");
                    await youtubeService.UploadVideo(videoPath, title, $"{description}\n\n{hashtags}", settings.Defaults.Hashtags);

                    System.Console.WriteLine("Done!\n");
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error: {ex.Message}\n");
                }
            };

            fileWatcher.Start(settings.WatchFolder);

            System.Console.WriteLine("\nPress any key to stop...");
            System.Console.ReadKey();
            
            fileWatcher.Stop();
        }
    }
}
