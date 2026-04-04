using Newtonsoft.Json;
using RestSharp;

namespace CSharer.Core.Services
{
    public class BufferService
    {
        private readonly string _accessToken;
        private readonly List<string> _profileIds;

        public BufferService(string accessToken, List<string> profileIds)
        {
            _accessToken = accessToken;
            _profileIds = profileIds;
        }

        public async Task<bool> UploadVideo(string videoPath, string caption, string hashtags = "")
        {
            try
            {
                var client = new RestClient("https://api.bufferapp.com/1");

                foreach (var profileId in _profileIds)
                {
                    var request = new RestRequest($"/updates/create.json?access_token={_accessToken}", Method.Post);
                    var fullCaption = $"{caption}\n\n{hashtags}";

                    request.AddParameter("profile_ids[]", profileId);
                    request.AddParameter("text", fullCaption);
                    request.AddParameter("media[video]", videoPath);

                    var response = await client.ExecuteAsync(request);

                    if (!response.IsSuccessful)
                    {
                        Console.WriteLine($"Buffer Error for profile {profileId}: {response.Content}");
                        return false;
                    }

                    Console.WriteLine($"Posted to Buffer profile: {profileId}");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Buffer Exception: {ex.Message}");
                return false;
            }
        }
    }
}