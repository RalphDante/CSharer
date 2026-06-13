using Newtonsoft.Json;
using RestSharp;

namespace CSharer.Core.Services
{
    public class AIService
    {
        private readonly string _apiKey;
        private readonly string _model;
        private readonly Random _random = new Random();

        private readonly string[] _titleFormulas = new[]
        {
            "The [Adjective] Way To [Achieve Result] Without [Common Pain]",
            "Why [Common Belief] Is Killing Your [Desired Outcome]",
            "Stop [Wrong Behavior] — Do This Instead",
            "What Top Students Know About [Topic] That You Don't",
            "[Unexpected Claim]: How I [Achieved Result] In [Short Time]",
            "[Number] [Specific Things] That [Result]"
        };

        public AIService(string apiKey, string model = "llama-3.3-70b-versatile")
        {
            _apiKey = apiKey;
            _model = model;
        }

        public async Task<string> GenerateStudyTitle(string extraRules = "")
        {
            var chosenFormula = _titleFormulas[_random.Next(_titleFormulas.Length)];

            var client = new RestClient("https://api.groq.com/openai/v1/chat/completions");
            var request = new RestRequest("", Method.Post);

            request.AddHeader("Authorization", $"Bearer {_apiKey}");
            request.AddHeader("Content-Type", "application/json");

            var prompt = $"You MUST follow these rules: {extraRules} " +
                $"You MUST use ONLY this headline formula and no other: '{chosenFormula}'. " +
                "Generate ONE unique, high-converting YouTube title for a study tips video. " +
                "Apply these copywriting principles from Jim Edwards' 'Copywriting Secrets': " +
                "1) Speak directly to a specific PAIN or DESIRE the viewer already has. " +
                "2) Promise a clear, specific, believable result — avoid vague words like 'better' or 'smarter'. " +
                "3) Use SPECIFICITY to build credibility (e.g. '3 hours', 'one page', '2 weeks', '5 steps'). " +
                "4) Create CURIOSITY or a pattern interrupt — the title should feel surprising or counterintuitive. " +
                "Topics: gamifying studying, focus, note-taking, procrastination, memory, exam prep, deep work. " +
                "Max 70 characters. No quotes. Return ONLY the title, nothing else.";

            var body = new
            {
                model = _model,
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 50,
                temperature = 0.9
            };

            request.AddJsonBody(body);
            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new Exception($"AI API Error: {response.Content}");

            dynamic? result = JsonConvert.DeserializeObject(response.Content ?? "");
            return result?.choices?[0]?.message?.content?.ToString()?.Trim() ?? "How To Study Smarter 📚";
        }

        public async Task<string> GenerateStudyDescription(string title, string extraRules = "")
        {
            var client = new RestClient("https://api.groq.com/openai/v1/chat/completions");
            var request = new RestRequest("", Method.Post);

            request.AddHeader("Authorization", $"Bearer {_apiKey}");
            request.AddHeader("Content-Type", "application/json");

            var prompt = $"You MUST follow these rules: {extraRules} " +
             $"Write a 2-3 sentence YouTube description for a study tips video titled: \"{title}\". " +
             "Be engaging, mention what viewers will learn, and end with a call to action. No hashtags.";

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

        public async Task<string> GenerateHashtags(string extraRules = "")
        {
            var client = new RestClient("https://api.groq.com/openai/v1/chat/completions");
            var request = new RestRequest("", Method.Post);

            request.AddHeader("Authorization", $"Bearer {_apiKey}");
            request.AddHeader("Content-Type", "application/json");

            var prompt = $"You MUST follow these rules: {extraRules} " +
             "Generate 5 YouTube hashtags for a study tips video. " +
             "Mix broad and niche tags. Examples of good ones: #StudyTips #HowToStudy #StudentLife #StudyMotivation #ExamPrep. " +
             "Return ONLY the hashtags separated by spaces, nothing else.";

            var body = new
            {
                model = _model,
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 50,
                temperature = 0.5
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