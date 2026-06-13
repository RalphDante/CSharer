using Newtonsoft.Json.Linq;
using RestSharp;
using CSharer.Core.Config;

namespace CSharer.Core.Services
{
    public class BufferService
    {
        private const string Endpoint = "https://api.buffer.com";

        private readonly string _apiKey;
        private readonly List<BufferChannel> _channels;

        public BufferService(string apiKey, List<BufferChannel> channels)
        {
            _apiKey = apiKey;
            _channels = channels;
        }

        // videoUrl must be a public URL (e.g. from Cloudinary), not a local file path
        public async Task<bool> UploadVideo(string videoUrl, string caption, string hashtags = "")
        {
            var fullCaption = string.IsNullOrWhiteSpace(hashtags) ? caption : $"{caption}\n\n{hashtags}";
            var client = new RestClient(Endpoint);
            var allSucceeded = true;

            const string query = @"
                mutation CreatePost($input: CreatePostInput!) {
                  createPost(input: $input) {
                    ... on PostActionSuccess {
                      post { id text }
                    }
                    ... on MutationError {
                      message
                    }
                  }
                }";

            foreach (var channel in _channels)
            {
                object input = channel.Service.ToLowerInvariant() switch
                {
                    "instagram" => new
                    {
                        text = fullCaption,
                        channelId = channel.Id,
                        schedulingType = "automatic",
                        mode = "customScheduled",
                        dueAt = DateTime.UtcNow.AddMinutes(2).ToString("o"),
                        assets = new object[]
                        {
                            new { video = new { url = videoUrl } }
                        },
                        metadata = new
                        {
                            instagram = new { type = "reel", shouldShareToFeed = true }
                        }
                    },
                    "facebook" => new
                    {
                        text = fullCaption,
                        channelId = channel.Id,
                        schedulingType = "automatic",
                        mode = "customScheduled",
                        dueAt = DateTime.UtcNow.AddMinutes(2).ToString("o"),
                        assets = new object[]
                        {
                            new { video = new { url = videoUrl } }
                        },
                        metadata = new
                        {
                            facebook = new { type = "reel" }
                        }
                    },
                    _ => new
                    {
                        text = fullCaption,
                        channelId = channel.Id,
                        schedulingType = "automatic",
                        mode = "customScheduled",
                        dueAt = DateTime.UtcNow.AddMinutes(2).ToString("o"),
                        assets = new object[]
                        {
                            new { video = new { url = videoUrl } }
                        }
                    }
                };

                var variables = new { input };

                var request = new RestRequest(string.Empty, Method.Post);
                request.AddHeader("Authorization", $"Bearer {_apiKey}");
                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(new { query, variables });

                var response = await client.ExecuteAsync(request);

                if (!response.IsSuccessful || response.Content is null)
                {
                    Console.WriteLine($"Buffer HTTP error for channel {channel.Id} ({channel.Service}): {response.StatusCode} {response.Content}");
                    allSucceeded = false;
                    continue;
                }

                var json = JObject.Parse(response.Content);

                var topLevelErrors = json["errors"];
                if (topLevelErrors != null && topLevelErrors.HasValues)
                {
                    Console.WriteLine($"Buffer GraphQL error for channel {channel.Id} ({channel.Service}): {topLevelErrors}");
                    allSucceeded = false;
                    continue;
                }

                var createPost = json["data"]?["createPost"];
                var mutationError = createPost?["message"];
                if (mutationError != null)
                {
                    Console.WriteLine($"Buffer mutation error for channel {channel.Id} ({channel.Service}): {mutationError}");
                    allSucceeded = false;
                    continue;
                }

                var postId = createPost?["post"]?["id"];
                Console.WriteLine($"Posted to Buffer channel {channel.Id} ({channel.Service}) (post id: {postId})");
            }

            return allSucceeded;
        }
    }
}