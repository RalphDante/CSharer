using Microsoft.Extensions.Configuration;
using CSharer.Core.Config;
using CSharer.Core.Services;
using CSharer.Core.Models;
using CSharer.Database;
using System.Linq;
using System.Windows.Forms;
using WinFormsApp2;

namespace CSharer.Console
{
    class Program
    {
        [STAThread]
        static async Task Main(string[] args)
        {
            try
            {
                var basePath = AppContext.BaseDirectory;
                var config = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                var settings = config.Get<AppSettings>()
                    ?? throw new Exception("Failed to load settings");

                var aiService = new AIService(settings.APIs.Groq.ApiKey, settings.APIs.Groq.Model);
                var youtubeService = new YouTubeUploadService(settings.APIs.YouTube.ClientSecretPath);
                var fileWatcher = new FileWatcherService();
                var videoRepo = new VideoPostRepository();

                fileWatcher.OnNewVideoDetected += async (videoPath) =>
                {
                    try
                    {
                        var titleRepo = new BaseParameterRepository("title_parameters");
                        var descRepo = new BaseParameterRepository("description_parameters");
                        var tagRepo = new BaseParameterRepository("hashtag_parameters");

                        var titleRules = string.Join(", ", titleRepo.GetAllActiveContents());
                        var descRules = string.Join(", ", descRepo.GetAllActiveContents());
                        var tagRules = string.Join(", ", tagRepo.GetAllActiveContents());

                        string title = "How To Study Smarter in 1 Hour ??";
                        string description = "Learn powerful study hacks to boost your focus and retention. Start improving today!";
                        string hashtags = "#StudyTips #Focus #StudentLife";

                        var fullCaption = $"{title}\n\n{description}\n\n{hashtags}";

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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"File watcher error: {ex.Message}", "Error");
                    }
                };

                var watcherThread = new Thread(() =>
                {
                    fileWatcher.Start(settings.WatchFolder);
                });
                watcherThread.IsBackground = true;
                watcherThread.Start();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());

                fileWatcher.Stop();
            }
            catch (Exception ex)
            {
                File.WriteAllText("C:\\Users\\christine\\Desktop\\crash.log", ex.ToString());
                MessageBox.Show(ex.ToString(), "Startup Error");
            }
        }
    }
}
