using System;
using System.Threading.Tasks;
using AI;

namespace FantasyWorldAI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Fantasy World AI Generator ===\n");

            // Choose service (comment/uncomment as needed)
            IAIService aiService;

            Console.WriteLine("Select AI Service:");
            Console.WriteLine("1. Local LLM Server (http://localhost:1234)");
            Console.WriteLine("2. Fallback (no LLM required)");
            Console.Write("Choice: ");

            string? choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Enter API URL [default: http://localhost:1234/v1/completions]: ");
                string? url = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(url))
                    url = "http://localhost:1234/v1/completions";

                aiService = new LocalLLMService(url);
                Console.WriteLine($"Using Local LLM at {url}");
            }
            else
            {
                aiService = new FallbackAIService();
                Console.WriteLine("Using Fallback Service");
            }

            Console.WriteLine("\n--- Testing Name Generation ---");

            // Generate dwarf names
            Console.WriteLine("\nGenerating Dwarf names:");
            string dwarfNames = await aiService.GenerateNamesAsync("dwarf", "mountain kingdom", 5);
            Console.WriteLine(dwarfNames);

            // Generate elf names
            Console.WriteLine("\nGenerating Elf names:");
            string elfNames = await aiService.GenerateNamesAsync("elf", "ancient forest", 5);
            Console.WriteLine(elfNames);

            // Test content generation
            Console.WriteLine("\n--- Testing Location Description ---");
            Console.WriteLine("Generating description for a haunted forest...");

            string locationPrompt = "Describe a haunted forest called 'Whispering Woods' in 3 sentences.";
            string locationDescription = await aiService.GenerateContentAsync(
                locationPrompt,
                "You are a fantasy world building assistant. Create vivid, immersive descriptions."
            );

            Console.WriteLine($"\nDescription:\n{locationDescription}");

            Console.WriteLine("\n--- Test Complete ---");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
