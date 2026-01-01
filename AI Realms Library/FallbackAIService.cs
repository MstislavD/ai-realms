namespace AI
{
    public class FallbackAIService : IAIService
    {
        private readonly Random _random = new Random();
        private readonly string[] _namePrefixes = { "Al", "Bar", "Cor", "Dan", "El", "Fen", "Gar", "Hal", "Ir", "Jor" };
        private readonly string[] _nameSuffixes = { "anor", "beth", "corin", "dor", "el", "fin", "gar", "horn", "ian", "jar" };

        public Task<string> GenerateContentAsync(string prompt, string systemPrompt = "")
        {
            return Task.FromResult($"Generated: {prompt.Substring(0, Math.Min(30, prompt.Length))}... [Fallback Mode]");
        }

        public Task<string> GenerateNamesAsync(string race, string culture, int count)
        {
            var names = GenerateNamesList(count);
            return Task.FromResult(string.Join("\n", names));
        }

        public Task<List<string>> GenerateNamesListAsync(string race, string culture, int count, string? additionalRequirements = null)
        {
            return Task.FromResult(GenerateNamesList(count));
        }

        public Task<Dictionary<string, string>> GenerateCharacterAsync(string race, string profession, string culture)
        {
            var names = GenerateNamesList(2);
            return Task.FromResult(new Dictionary<string, string>
            {
                ["Name"] = names[0] + " " + names[1],
                ["Backstory"] = $"A {race} {profession} from {culture}. Has seen many things in their travels.",
                ["Trait"] = GetRandomTrait(),
                ["Secret"] = GetRandomSecret()
            });
        }

        private List<string> GenerateNamesList(int count)
        {
            var names = new List<string>();
            for (int i = 0; i < count; i++)
            {
                string name = _namePrefixes[_random.Next(_namePrefixes.Length)] +
                             _nameSuffixes[_random.Next(_nameSuffixes.Length)];
                names.Add(name);
            }
            return names;
        }

        private string GetRandomTrait()
        {
            string[] traits = { "Brave", "Cunning", "Wise", "Charismatic", "Observant", "Patient" };
            return traits[_random.Next(traits.Length)];
        }

        private string GetRandomSecret()
        {
            string[] secrets = { "Former assassin", "Royal lineage", "Cursed artifact bearer", "Spy for rival faction" };
            return secrets[_random.Next(secrets.Length)];
        }
    }
}
