using System.Text;
using Newtonsoft.Json;

namespace AI
{
    public class RobustAIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly int _maxRetries;

        public RobustAIService(string apiUrl = "http://localhost:1234/v1/chat/completions", int maxRetries = 2)
        {
            _httpClient = new HttpClient();
            _apiUrl = apiUrl;
            _maxRetries = maxRetries;
            _httpClient.Timeout = TimeSpan.FromSeconds(45);
        }

        public async Task<string> GenerateContentAsync(string prompt, string systemPrompt = "")
        {
            return await GenerateContentInternalAsync(prompt, systemPrompt);
        }

        // Original interface method - returns string
        public async Task<string> GenerateNamesAsync(string race, string culture, int count)
        {
            var names = await GenerateNamesListAsync(race, culture, count);
            return string.Join("\n", names);
        }

        // Enhanced method - returns list
        public async Task<List<string>> GenerateNamesListAsync(string race, string culture, int count, string? additionalRequirements = null)
        {
            string prompt = PromptTemplates.GenerateNamesPrompt(race, culture, count, additionalRequirements);
            string systemPrompt = PromptTemplates.NameGeneratorSystem;

            string response = await GenerateContentInternalAsync(prompt, systemPrompt);
            return ResponseParser.ParseNamesResponse(response, count);
        }

        // Structured generation
        public async Task<Dictionary<string, string>> GenerateCharacterAsync(string race, string profession, string culture)
        {
            var prompt = $"Generate a {race} {profession} from {culture} culture. " +
                        $"Provide:\n1. Full name\n2. Brief backstory (2 sentences)\n" +
                        $"3. Personality trait\n4. Secret or quirk\n" +
                        $"Format as: Name: [name]\nBackstory: [text]\nTrait: [text]\nSecret: [text]";

            var response = await GenerateContentInternalAsync(prompt, PromptTemplates.WorldBuilderSystem);

            return ParseCharacterResponse(response);
        }

        // Internal implementation with retry logic
        private async Task<string> GenerateContentInternalAsync(string prompt, string systemPrompt = "")
        {
            for (int attempt = 0; attempt <= _maxRetries; attempt++)
            {
                try
                {
                    // Use chat format for better instruction following
                    var messages = new List<object>();

                    if (!string.IsNullOrEmpty(systemPrompt))
                    {
                        messages.Add(new { role = "system", content = systemPrompt });
                    }

                    messages.Add(new { role = "user", content = prompt });

                    var request = new
                    {
                        messages = messages,
                        max_tokens = 300,
                        temperature = 0.7,
                        top_p = 0.9,
                        frequency_penalty = 0.3,
                        presence_penalty = 0.3,
                        stop = new[] { "\n\n", "###", "<|end|>" }
                    };

                    var json = JsonConvert.SerializeObject(request);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync(_apiUrl, content);
                    response.EnsureSuccessStatusCode();

                    var responseJson = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(responseJson);

                    string rawResponse = result.choices[0].message.content.ToString();

                    // Clean and validate
                    string cleaned = ResponseParser.CleanResponse(rawResponse);

                    if (!string.IsNullOrWhiteSpace(cleaned) && !cleaned.Contains("ERROR"))
                        return cleaned;

                    // If response is invalid, retry with stricter prompt
                    if (attempt < _maxRetries)
                    {
                        prompt = $"{prompt}\n\nIMPORTANT: Respond ONLY with the requested content, NO explanations.";
                    }
                }
                catch (Exception ex)
                {
                    if (attempt == _maxRetries)
                        return $"AI Generation Failed: {ex.Message}";

                    await Task.Delay(1000 * (attempt + 1));
                }
            }

            return "Failed to generate content after multiple attempts.";
        }

        private Dictionary<string, string> ParseCharacterResponse(string response)
        {
            var result = new Dictionary<string, string>();
            var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (line.Contains(':'))
                {
                    var parts = line.Split(':', 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();

                        // Handle common key variations
                        if (key.Equals("Full name", StringComparison.OrdinalIgnoreCase) ||
                            key.Equals("Name", StringComparison.OrdinalIgnoreCase))
                            result["Name"] = value;
                        else if (key.Equals("Backstory", StringComparison.OrdinalIgnoreCase) ||
                                 key.Equals("Background", StringComparison.OrdinalIgnoreCase))
                            result["Backstory"] = value;
                        else if (key.Equals("Trait", StringComparison.OrdinalIgnoreCase) ||
                                 key.Equals("Personality", StringComparison.OrdinalIgnoreCase))
                            result["Trait"] = value;
                        else if (key.Equals("Secret", StringComparison.OrdinalIgnoreCase) ||
                                 key.Equals("Quirk", StringComparison.OrdinalIgnoreCase))
                            result["Secret"] = value;
                        else
                            result[key] = value;
                    }
                }
            }

            // Ensure all required fields exist
            if (!result.ContainsKey("Name")) result["Name"] = "Unknown";
            if (!result.ContainsKey("Backstory")) result["Backstory"] = "No backstory generated.";
            if (!result.ContainsKey("Trait")) result["Trait"] = "Mysterious";
            if (!result.ContainsKey("Secret")) result["Secret"] = "Has a hidden past.";

            return result;
        }
    }
}
