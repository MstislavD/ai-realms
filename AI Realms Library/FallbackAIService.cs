namespace AI
{
    public class FallbackAIService : IAIService
    {
        private readonly Random _random = new Random();
        private readonly string[] _namePrefixes = { "Al", "Bar", "Cor", "Dan", "El", "Fen", "Gar", "Hal", "Ir", "Jor" };
        private readonly string[] _nameSuffixes = { "anor", "beth", "corin", "dor", "el", "fin", "gar", "horn", "ian", "jar" };

        public Task<string> GenerateContentAsync(string prompt, string context = "")
        {
            // Simple fallback for testing without LLM
            return Task.FromResult($"Generated content for: {prompt.Substring(0, Math.Min(30, prompt.Length))}... [Fallback Mode]");
        }

        public Task<string> GenerateNamesAsync(string race, string culture, int count)
        {
            var names = new System.Text.StringBuilder();
            for (int i = 0; i < count; i++)
            {
                string name = _namePrefixes[_random.Next(_namePrefixes.Length)] +
                             _nameSuffixes[_random.Next(_nameSuffixes.Length)];
                names.AppendLine(name);
            }
            return Task.FromResult(names.ToString());
        }
    }
}
