using Newtonsoft.Json;
using RestSharp;

namespace CSharer.Core.Services
{
    public class AIService
    {
        private readonly string _apiKey;
        private readonly string _model;

        public AIService(string apiKey, string model = "llama-3.3-70b-versatile")
        {
            _apiKey = apiKey;
            _model = model;
        }

        public async Task<string> GenerateStudyTitle()
        {
            var client = new RestClient("https://api.groq.com/openai/v1/chat/completions");
            var request = new RestRequest("", Method.Post);
            
            request.AddHeader("Authorization", $"Bearer {_apiKey}");
            request.AddHeader("Content-Type", "application/json");

            var prompt = "Generate ONE unique, viral YouTube title for a study tips video. " +
                 "Topics can include: gamifying studying, focus techniques, note-taking methods, " +
                 "beating procrastination, memory tricks, study schedules, or exam prep. " +
                 "Format: 'How To [Action] [Benefit]' or similar. Keep the full title under 70 characters. " +
                 "End the title with 1-2 relevant hashtags like #StudyTips #StudentLife. " +
                 "No quotes. Just the title with hashtags at the end.";

            var body = new
            {
                model = _model,
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 50,
                temperature = 0.9  // Higher = more variety each run
            };

            request.AddJsonBody(body);
            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new Exception($"AI API Error: {response.Content}");

            dynamic? result = JsonConvert.DeserializeObject(response.Content ?? "");
            return result?.choices?[0]?.message?.content?.ToString()?.Trim() ?? "How To Study Smarter 📚";
        }

        public async Task<string> GenerateStudyDescription(string title)
        {
            var client = new RestClient("https://api.groq.com/openai/v1/chat/completions");
            var request = new RestRequest("", Method.Post);

            request.AddHeader("Authorization", $"Bearer {_apiKey}");
            request.AddHeader("Content-Type", "application/json");

            var prompt = $"Write a 2-3 sentence YouTube description for a study tips video titled: \"{title}\". " +
                        "Be engaging, mention what viewers will learn, and end with a call to action. No hashtags. " +
                        "Include the link 'mastery-study.web.app'";

            var body = new
            {
                model = _model,
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 120,
                temperature = 0.7
            };

            request.AddJsonBody(body);
            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new Exception($"AI API Error: {response.Content}");

            dynamic? result = JsonConvert.DeserializeObject(response.Content ?? "");
            return result?.choices?[0]?.message?.content?.ToString()?.Trim() ?? "Watch till the end to level up your study game!";
        }

        public async Task<string> GenerateHashtags()
        {
            var client = new RestClient("https://api.groq.com/openai/v1/chat/completions");
            var request = new RestRequest("", Method.Post);

            request.AddHeader("Authorization", $"Bearer {_apiKey}");
            request.AddHeader("Content-Type", "application/json");

            var prompt = "Generate 5 YouTube hashtags for a study tips video. " +
                        "Mix broad and niche tags. Examples of good ones: #StudyTips #HowToStudy #StudentLife #StudyMotivation #ExamPrep. " +
                        "Return ONLY the hashtags separated by spaces, nothing else.";

            var body = new
            {
                model = _model,
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 50,
                temperature = 0.5  // Lower = more consistent, reliable hashtags
            };

            request.AddJsonBody(body);
            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new Exception($"AI API Error: {response.Content}");

            dynamic? result = JsonConvert.DeserializeObject(response.Content ?? "");
            return result?.choices?[0]?.message?.content?.ToString()?.Trim() ?? "#StudyTips #HowToStudy #StudentLife";
        }
    }
}
