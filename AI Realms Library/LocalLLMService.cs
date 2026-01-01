using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AI
{
    public class LocalLLMService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public LocalLLMService(string apiUrl = "http://localhost:1234/v1/completions")
        {
            _httpClient = new HttpClient();
            _apiUrl = apiUrl;
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public Task<Dictionary<string, string>> GenerateCharacterAsync(string race, string profession, string culture)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GenerateContentAsync(string prompt, string context = "")
        {
            string fullPrompt = context + "\n\n" + prompt;

            var request = new
            {
                prompt = fullPrompt,
                max_tokens = 150,
                temperature = 0.7,
                stop = new[] { "\n\n", "---" }
            };

            string json = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(_apiUrl, content);
                response.EnsureSuccessStatusCode();

                string responseJson = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseJson);

                return result.choices[0].text.ToString().Trim();
            }
            catch (Exception ex)
            {
                return $"AI Error: {ex.Message}. Fallback: Generated content for '{prompt.Substring(0, Math.Min(50, prompt.Length))}...'";
            }
        }

        public async Task<string> GenerateNamesAsync(string race, string culture, int count)
        {
            string prompt = $"Generate {count} unique fantasy names for a {race} from {culture} culture. " +
                           $"Names should be pronounceable and fitting for the setting. " +
                           $"Return only the names, one per line, no numbering.";

            return await GenerateContentAsync(prompt, "You are a fantasy name generator.");
        }

        public Task<List<string>> GenerateNamesListAsync(string race, string culture, int count, string? additionalRequirements = null)
        {
            throw new NotImplementedException();
        }
    }
}

