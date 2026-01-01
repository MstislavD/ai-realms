using System;
using System.Threading.Tasks;
using AI;

namespace FantasyWorldAI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Fantasy World AI Generator (Fixed Version) ===\n");

            IAIService aiService = CreateAIService();

            // Test basic interface method
            Console.WriteLine("\n--- Testing Basic Interface Method ---");
            Console.WriteLine("Generating Dwarf names (string output):");
            string dwarfNamesString = await aiService.GenerateNamesAsync("dwarf", "mountain kingdom", 3);
            Console.WriteLine(dwarfNamesString);

            // Test enhanced method if available
            if (aiService is RobustAIService robustService)
            {
                Console.WriteLine("\n--- Testing Enhanced Methods ---");

                // Test list generation
                Console.WriteLine("\nGenerating Elf names (list output):");
                var elfNames = await robustService.GenerateNamesListAsync(
                    "elf",
                    "ancient forest",
                    4,
                    "Names should be melodic"
                );

                Console.WriteLine("Generated Names:");
                foreach (var name in elfNames)
                {
                    Console.WriteLine($"  • {name}");
                }

                // Test character generation
                Console.WriteLine("\nGenerating a character:");
                var character = await robustService.GenerateCharacterAsync(
                    "human",
                    "merchant",
                    "desert nomad"
                );

                Console.WriteLine("\nCharacter Details:");
                Console.WriteLine($"Name: {character["Name"]}");
                Console.WriteLine($"Backstory: {character["Backstory"]}");
                Console.WriteLine($"Trait: {character["Trait"]}");
                Console.WriteLine($"Secret: {character["Secret"]}");
            }

            // Test content generation
            Console.WriteLine("\n--- Testing Content Generation ---");
            var locationDesc = await aiService.GenerateContentAsync(
                "Describe a magical fountain in a hidden glade in 2 sentences.",
                "You are a fantasy describer. Be vivid but concise."
            );

            Console.WriteLine($"\nLocation Description:\n{locationDesc}");

            Console.WriteLine("\n=== Test Complete ===");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static IAIService CreateAIService()
        {
            Console.WriteLine("Select AI Service:");
            Console.WriteLine("1. Robust AI Service (Chat endpoint)");
            Console.WriteLine("2. Legacy Service (Completions endpoint)");
            Console.WriteLine("3. Fallback Service (No LLM required)");
            Console.Write("Your choice: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter API URL [default: http://localhost:1234/v1/chat/completions]: ");
                    var url = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(url))
                        url = "http://localhost:1234/v1/chat/completions";

                    Console.Write("Max retries [default: 2]: ");
                    if (int.TryParse(Console.ReadLine(), out int retries))
                        return new RobustAIService(url, retries);
                    else
                        return new RobustAIService(url);

                case "2":
                    // For legacy completions endpoint
                    Console.Write("Enter API URL: ");
                    var legacyUrl = Console.ReadLine();
                    return new LocalLLMService(legacyUrl);

                case "3":
                default:
                    return new FallbackAIService();
            }
        }
    }
}
