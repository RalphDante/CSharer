using Newtonsoft.Json;
using RestSharp;

namespace CSharer.Core.Services
{
    public class PinterestService
    {
        private readonly string _accessToken;
        private readonly string _boardId;

        public PinterestService(string accessToken, string boardId)
        {
            _accessToken = accessToken;
            _boardId = boardId;
        }

        public async Task<bool> UploadVideoPin(string videoPath, string caption)
        {
            try
            {
                var client = new RestClient("https://api.pinterest.com/v5");
                var request = new RestRequest("/pins", Method.Post);

                request.AddHeader("Authorization", $"Bearer {_accessToken}");
                request.AddHeader("Content-Type", "application/json");

                var body = new
                {
                    board_id = _boardId,  // Fixed: was "board_idC" typo
                    title = caption,
                    description = caption,
                    media_source = new
                    {
                        source_type = "video_url",
                        url = videoPath // Note: Pinterest needs a URL, not local file
                    }
                };

                request.AddJsonBody(body);
                var response = await client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    Console.WriteLine($"Pinterest Error: {response.Content}");
                    return false;
                }

                Console.WriteLine("✓ Posted to Pinterest");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pinterest Exception: {ex.Message}");
                return false;
            }
        }
    }
}