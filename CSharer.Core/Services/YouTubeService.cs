using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace CSharer.Core.Services
{
    public class YouTubeUploadService
    {
        private readonly string _clientSecretPath;

        public YouTubeUploadService(string clientSecretPath)
        {
            _clientSecretPath = clientSecretPath;
        }

        public async Task<bool> UploadVideo(string videoPath, string title, string description, string tags = "")
        {
            try
            {
                UserCredential credential;
                using (var stream = new FileStream(_clientSecretPath, FileMode.Open, FileAccess.Read))
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        new[] { Google.Apis.YouTube.v3.YouTubeService.Scope.YoutubeUpload },
                        "user",
                        CancellationToken.None
                    );
                }

                var youtubeService = new Google.Apis.YouTube.v3.YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "CSharer"
                });

                var video = new Video
                {
                    Snippet = new VideoSnippet
                    {
                        Title = title,
                        Description = description,
                        Tags = string.IsNullOrEmpty(tags) ? null : tags.Split(' '),
                        CategoryId = "22" // People & Blogs
                    },
                    Status = new VideoStatus
                    {
                        PrivacyStatus = "public" // Change to "public" when ready
                    }
                };

                using var fileStream = new FileStream(videoPath, FileMode.Open);
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                
                videosInsertRequest.ProgressChanged += progress =>
                {
                    switch (progress.Status)
                    {
                        case UploadStatus.Uploading:
                            Console.WriteLine($"Uploading: {progress.BytesSent} bytes");
                            break;
                        case UploadStatus.Failed:
                            Console.WriteLine($"Upload failed: {progress.Exception}");
                            break;
                    }
                };

                videosInsertRequest.ResponseReceived += video =>
                {
                    Console.WriteLine($"Video uploaded! ID: {video.Id}");
                    Console.WriteLine($"URL: https://www.youtube.com/watch?v={video.Id}");
                };

                var uploadResponse = await videosInsertRequest.UploadAsync();

                if (uploadResponse.Status == UploadStatus.Completed)
                {
                    Console.WriteLine("Uploaded to YouTube successfully");
                    return true;
                }
                else
                {
                    Console.WriteLine($"YouTube upload failed: {uploadResponse.Exception?.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"YouTube Exception: {ex.Message}");
                return false;
            }
        }
    }
}
