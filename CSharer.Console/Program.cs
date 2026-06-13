using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestSharp;
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

            // One-off helper: dotnet run -- --list-channels
            if (args.Contains("--list-channels"))
            {
                await ListBufferChannels(settings.APIs.Buffer.ApiKey);
                return;
            }

            // Initialize services
            var aiService = new AIService(settings.APIs.Groq.ApiKey, settings.APIs.Groq.Model);
            var youtubeService = new YouTubeUploadService(settings.APIs.YouTube.ClientSecretPath);
            var cloudinaryService = new CloudinaryService(
                settings.APIs.Cloudinary.CloudName,
                settings.APIs.Cloudinary.ApiKey,
                settings.APIs.Cloudinary.ApiSecret);
            var bufferService = new BufferService(
                settings.APIs.Buffer.ApiKey,
                settings.APIs.Buffer.Channels);
            var fileWatcher = new FileWatcherService();

            // Database repository
            // var videoRepo = new VideoPostRepository();

            fileWatcher.OnNewVideoDetected += async (videoPath) =>
            {
                try
                {
                    System.Console.WriteLine($"\nGenerating metadata...");

                    //  GET PARAMETERS FROM DB (3 TABLES)
                    // var titleRepo = new BaseParameterRepository("title_parameters");
                    // var descRepo = new BaseParameterRepository("description_parameters");
                    // var tagRepo = new BaseParameterRepository("hashtag_parameters");

                    var titleRules = "This channel targets college students aged 18-24. Avoid titles that sound corporate or generic.";
                    var descRules = "Include the link 'https://mastery-study.web.app'";
                    var tagRules = "";

                    // Debug output (check DB data)
                    System.Console.WriteLine("Title Rules: " + titleRules);
                    System.Console.WriteLine("Description Rules: " + descRules);
                    System.Console.WriteLine("Hashtag Rules: " + tagRules);

                    // AI GENERATION
                    var title = await aiService.GenerateStudyTitle(titleRules);
                    var description = await aiService.GenerateStudyDescription(title, descRules);
                    var hashtags = await aiService.GenerateHashtags(tagRules);

                    // TEST TEST RA (NO API KEY YET)
                    // string title = "How To Study Smarter in 1 Hour 📚";
                    // string description = "Learn powerful study hacks to boost your focus and retention. Start improving today!";
                    // string hashtags = "#StudyTips #Focus #StudentLife";

                    // System.Console.WriteLine("⚠️ Using MOCK AI (no API key)");

                    // OUTPUT
                    System.Console.WriteLine($"\nTitle: {title}");
                    System.Console.WriteLine($"Description: {description}");
                    System.Console.WriteLine($"Hashtags: {hashtags}");

                    var fullCaption = $"{title}\n\n{description}\n\n{hashtags}";

                    // SAVE TO DATABASE
                    // var videoPost = new VideoPost
                    // {
                    //     VideoPath = videoPath,
                    //     FileName = Path.GetFileName(videoPath),
                    //     GeneratedCaption = fullCaption,
                    //     DetectedAt = DateTime.Now,
                    //     PostedToBuffer = false,
                    //     PostedToPinterest = false
                    // };

                    // videoRepo.AddVideoPost(videoPost);
                    // System.Console.WriteLine("Saved to database.");

                    // YOUTUBE UPLOAD (OPTIONAL)

                    await youtubeService.UploadVideo(
                        videoPath,
                        title,
                        $"{description}\n\n{hashtags}",
                        settings.Defaults.Hashtags
                    );

                    // BUFFER UPLOAD (via Cloudinary-hosted URL)
                    System.Console.WriteLine("\nUploading video to Cloudinary...");
                    var videoUrl = await cloudinaryService.UploadVideo(videoPath);
                    System.Console.WriteLine($"Video URL: {videoUrl}");

                    System.Console.WriteLine("\nPosting to Buffer...");
                    var bufferOk = await bufferService.UploadVideo(videoUrl, title, $"{description}\n\n{hashtags}");
                    System.Console.WriteLine(bufferOk
                        ? "Buffer post created."
                        : "Buffer post failed for one or more channels.");

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

        // One-off helper to discover organization + channel IDs for appsettings.json
        static async Task ListBufferChannels(string apiKey)
        {
            var client = new RestClient("https://api.buffer.com");

            // Step 1: organizations
            var orgRequest = new RestRequest(string.Empty, Method.Post);
            orgRequest.AddHeader("Authorization", $"Bearer {apiKey}");
            orgRequest.AddHeader("Content-Type", "application/json");
            orgRequest.AddJsonBody(new
            {
                query = @"
                    query GetOrganizations {
                      account {
                        organizations {
                          id
                          name
                          ownerEmail
                        }
                      }
                    }"
            });

            var orgResponse = await client.ExecuteAsync(orgRequest);
            if (!orgResponse.IsSuccessful || orgResponse.Content is null)
            {
                System.Console.WriteLine($"Failed to fetch organizations: {orgResponse.StatusCode} {orgResponse.Content}");
                return;
            }

            var orgJson = JObject.Parse(orgResponse.Content);
            var organizations = orgJson["data"]?["account"]?["organizations"];

            if (organizations is null || !organizations.HasValues)
            {
                System.Console.WriteLine("No organizations found for this API key.");
                return;
            }

            foreach (var org in organizations)
            {
                var orgId = org["id"]?.ToString();
                var orgName = org["name"]?.ToString();

                System.Console.WriteLine($"\nOrganization: {orgName} ({orgId})");

                // Step 2: channels for this organization
                var channelsRequest = new RestRequest(string.Empty, Method.Post);
                channelsRequest.AddHeader("Authorization", $"Bearer {apiKey}");
                channelsRequest.AddHeader("Content-Type", "application/json");
                channelsRequest.AddJsonBody(new
                {
                    query = @"
                        query GetChannels($organizationId: OrganizationId!) {
                        channels(input: { organizationId: $organizationId }) {
                            id
                            name
                            displayName
                            service
                        }
                    }",
                    variables = new { organizationId = orgId }
                });

                var channelsResponse = await client.ExecuteAsync(channelsRequest);
                if (!channelsResponse.IsSuccessful || channelsResponse.Content is null)
                {
                    System.Console.WriteLine($"  Failed to fetch channels: {channelsResponse.StatusCode} {channelsResponse.Content}");
                    continue;
                }

                var channelsJson = JObject.Parse(channelsResponse.Content);
                var channels = channelsJson["data"]?["channels"];

                if (channels is null || !channels.HasValues)
                {
                    System.Console.WriteLine("  No connected channels.");
                    continue;
                }

                foreach (var channel in channels)
                {
                    var id = channel["id"]?.ToString();
                    var service = channel["service"]?.ToString();
                    var displayName = channel["displayName"]?.ToString() ?? channel["name"]?.ToString();

                    System.Console.WriteLine($"  [{service}] {displayName} -> {id}");
                }
            }
        }
    }
}