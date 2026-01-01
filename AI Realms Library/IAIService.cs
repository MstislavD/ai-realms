using System.Collections.Generic;
using System.Threading.Tasks;

namespace AI
{
    public interface IAIService
    {
        Task<string> GenerateContentAsync(string prompt, string systemPrompt = "");
        Task<string> GenerateNamesAsync(string race, string culture, int count);

        // Optional methods for enhanced functionality
        Task<List<string>> GenerateNamesListAsync(string race, string culture, int count, string? additionalRequirements = null);
        Task<Dictionary<string, string>> GenerateCharacterAsync(string race, string profession, string culture);
    }
}

