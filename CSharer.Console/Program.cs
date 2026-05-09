// using Microsoft.Extensions.Configuration;
// using CSharer.Core.Config;
// using CSharer.Core.Services;

// namespace CSharer.Console
// {
//     class Program
//     {
//         static async Task Main(string[] args)
//         {
//             System.Console.WriteLine("CSharer - YouTube Auto-Uploader\n");

//             var basePath = AppContext.BaseDirectory;
//             var config = new ConfigurationBuilder()
//                 .SetBasePath(basePath)
//                 .AddJsonFile("appsettings.json", optional: false)
//                 .Build();

//             var settings = config.Get<AppSettings>() ?? throw new Exception("Failed to load settings");

//             // Initialize services
//             var aiService = new AIService(settings.APIs.Groq.ApiKey, settings.APIs.Groq.Model);
//             var youtubeService = new YouTubeUploadService(settings.APIs.YouTube.ClientSecretPath);
//             var fileWatcher = new FileWatcherService();

//             fileWatcher.OnNewVideoDetected += async (videoPath) =>
//             {
//                 try
//                 {
//                     System.Console.WriteLine($"\nGenerating metadata...");

//                     var title = await aiService.GenerateStudyTitle();
//                     var description = await aiService.GenerateStudyDescription(title);
//                     var hashtags = await aiService.GenerateHashtags();

//                     System.Console.WriteLine($"Title: {title}");
//                     System.Console.WriteLine($"Description: {description}");
//                     System.Console.WriteLine($"Hashtags: {hashtags}");

//                     System.Console.WriteLine("\nUploading to YouTube...");
//                     await youtubeService.UploadVideo(videoPath, title, $"{description}\n\n{hashtags}", settings.Defaults.Hashtags);

//                     System.Console.WriteLine("Done!\n");
//                 }
//                 catch (Exception ex)
//                 {
//                     System.Console.WriteLine($"Error: {ex.Message}\n");
//                 }
//             };

//             fileWatcher.Start(settings.WatchFolder);

//             System.Console.WriteLine("\nPress any key to stop...");
//             System.Console.ReadKey();
            
//             fileWatcher.Stop();
//         }
//     }
// }

using Microsoft.Extensions.Configuration;
using CSharer.Core.Config;
using CSharer.Core.Services;
using CSharer.Core.Models;
using CSharer.Database;
using System.Linq;

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

            var settings = config.Get<AppSettings>() 
                ?? throw new Exception("Failed to load settings");

            // Initialize services
            var aiService = new AIService(settings.APIs.Groq.ApiKey, settings.APIs.Groq.Model);
            var youtubeService = new YouTubeUploadService(settings.APIs.YouTube.ClientSecretPath);
            var fileWatcher = new FileWatcherService();

            // Database repository
            var videoRepo = new VideoPostRepository();

            fileWatcher.OnNewVideoDetected += async (videoPath) =>
            {
                try
                {
                    System.Console.WriteLine($"\nGenerating metadata...");

                    //  GET PARAMETERS FROM DB (3 TABLES)
                    var titleRepo = new BaseParameterRepository("title_parameters");
                    var descRepo = new BaseParameterRepository("description_parameters");
                    var tagRepo = new BaseParameterRepository("hashtag_parameters");

                    var titleRules = string.Join(", ", titleRepo.GetAllActiveContents());
                    var descRules = string.Join(", ", descRepo.GetAllActiveContents());
                    var tagRules = string.Join(", ", tagRepo.GetAllActiveContents());

                    // Debug output (check DB data)
                    System.Console.WriteLine("Title Rules: " + titleRules);
                    System.Console.WriteLine("Description Rules: " + descRules);
                    System.Console.WriteLine("Hashtag Rules: " + tagRules);

                    // AI GENERATION
                    // var title = await aiService.GenerateStudyTitle(titleRules);
                    // var description = await aiService.GenerateStudyDescription(title, descRules);
                    // var hashtags = await aiService.GenerateHashtags(tagRules);

                    // TEST TEST RA (NO API KEY YET)
                    string title = "How To Study Smarter in 1 Hour 📚";
                    string description = "Learn powerful study hacks to boost your focus and retention. Start improving today!";
                    string hashtags = "#StudyTips #Focus #StudentLife";

                    System.Console.WriteLine("⚠️ Using MOCK AI (no API key)");

                    // OUTPUT
                    System.Console.WriteLine($"\nTitle: {title}");
                    System.Console.WriteLine($"Description: {description}");
                    System.Console.WriteLine($"Hashtags: {hashtags}");

                    var fullCaption = $"{title}\n\n{description}\n\n{hashtags}";

                    // SAVE TO DATABASE
                    var videoPost = new VideoPost
                    {
                        VideoPath = videoPath,
                        FileName = Path.GetFileName(videoPath),
                        GeneratedCaption = fullCaption,
                        DetectedAt = DateTime.Now,
                        PostedToBuffer = false,
                        PostedToPinterest = false
                    };

                    videoRepo.AddVideoPost(videoPost);
                    System.Console.WriteLine("Saved to database.");

                    // YOUTUBE UPLOAD (OPTIONAL)
                    
                    await youtubeService.UploadVideo(
                        videoPath,
                        title,
                        $"{description}\n\n{hashtags}",
                        settings.Defaults.Hashtags
                    );
                    

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