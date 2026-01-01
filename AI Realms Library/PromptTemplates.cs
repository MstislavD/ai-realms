using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI
{
    public static class PromptTemplates
    {
        // System prompts for consistent behavior
        public const string NameGeneratorSystem =
            "You are a fantasy name generator. You ONLY return the requested names, " +
            "one per line, with NO numbering, explanations, or additional text. " +
            "If you cannot generate a name, return 'ERROR' on its own line.";

        public const string WorldBuilderSystem =
            "You are a fantasy world-building assistant. You create vivid, concise descriptions " +
            "following exactly the requested format. Do not add introductory phrases.";

        // Structured prompts with placeholders
        public static string GenerateNamesPrompt(string race, string culture, int count, string? additionalRequirements = null)
        {
            string prompt =
                $"Generate {count} unique fantasy names for {race} from {culture} culture. " +
                $"Names should be pronounceable and culturally appropriate. " +
                $"Format: One name per line, no numbering.";

            if (!string.IsNullOrEmpty(additionalRequirements))
                prompt += $" Additional requirements: {additionalRequirements}";

            return prompt;
        }

        public static string GenerateLocationPrompt(string locationName, string biome, string[] tags, int sentences = 3)
        {
            string tagsStr = tags.Length > 0 ? $"Tags: {string.Join(", ", tags)}. " : "";
            return $"Describe '{locationName}', a {biome}. {tagsStr}" +
                   $"Write exactly {sentences} sentences. Be vivid but concise.";
        }
    }
}
