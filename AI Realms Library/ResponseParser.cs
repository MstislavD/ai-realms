using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AI
{
    public static class ResponseParser
    {
        private static readonly Regex[] _cleanupPatterns = {
            new Regex(@"^[^a-zA-Z0-9]*"), // Remove leading non-alphanumeric
            new Regex(@"[^a-zA-Z0-9\s\.\-\']*$"), // Remove trailing junk
            new Regex(@"\s+"), // Normalize whitespace
            new Regex(@"(?i)(here (are|is) (the )?(names|description):)"), // Remove intro phrases
            new Regex(@"(?i)(as (requested|specified):)"),
            new Regex(@"(?i)^\d+[\.\)]\s*"), // Remove numbering
        };

        public static string CleanResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                return "No response generated.";

            string cleaned = response;

            // Remove common LLM analysis/thinking sections
            Match thinkingMatch = Regex.Match(cleaned, @"<\|[^>]+\|>");
            if (thinkingMatch.Success)
            {
                // Extract only the final assistant message
                Match finalMatch = Regex.Match(cleaned, @"<\|start\|>assistant<\|channel\|>final<\|message\|>(.*?)<\|end\|>",
                    RegexOptions.Singleline);
                if (finalMatch.Success)
                    cleaned = finalMatch.Groups[1].Value;
            }

            // Apply cleanup patterns
            foreach (Regex pattern in _cleanupPatterns)
            {
                cleaned = pattern.Replace(cleaned, " ");
            }

            // Split into lines and clean each
            string[] lines = cleaned.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
            List<string> cleanedLines = new List<string>();

            foreach (var line in lines)
            {
                string lineTrimmed = line.Trim();
                if (lineTrimmed.Length > 0 &&
                    !lineTrimmed.StartsWith("I'll", StringComparison.OrdinalIgnoreCase) &&
                    !lineTrimmed.StartsWith("Sure", StringComparison.OrdinalIgnoreCase) &&
                    !lineTrimmed.StartsWith("Okay", StringComparison.OrdinalIgnoreCase) &&
                    !lineTrimmed.StartsWith("Here", StringComparison.OrdinalIgnoreCase) &&
                    !lineTrimmed.StartsWith("As ", StringComparison.OrdinalIgnoreCase) &&
                    lineTrimmed.Length > 2) // Filter out short junk
                {
                    cleanedLines.Add(lineTrimmed);
                }
            }

            return string.Join("\n", cleanedLines);
        }

        public static List<string> ParseNamesResponse(string response, int expectedCount)
        {
            var cleaned = CleanResponse(response);
            var lines = cleaned.Split(['\n'], StringSplitOptions.RemoveEmptyEntries)
                              .Select(l => l.Trim())
                              .Where(l => !string.IsNullOrWhiteSpace(l))
                              .ToList();

            // If we got exactly the expected count, return them
            if (lines.Count == expectedCount)
                return lines;

            // If we got more, take the first expectedCount
            if (lines.Count > expectedCount)
                return lines.Take(expectedCount).ToList();

            // If we got fewer, generate fallback names
            if (lines.Count < expectedCount && lines.Count > 0)
            {
                // Duplicate and modify existing names if needed
                while (lines.Count < expectedCount)
                {
                    string name = lines[lines.Count % lines.Count];
                    // Add a suffix to make it unique
                    lines.Add(name + " II");
                }
                return lines;
            }

            // Complete fallback
            return GenerateFallbackNames(expectedCount);
        }

        private static List<string> GenerateFallbackNames(int count)
        {
            string[] prefixes = ["Al", "Bar", "Cor", "Dan", "El", "Fen", "Gar", "Hal", "Ir"];
            string[] suffixes = ["anor", "beth", "corin", "dor", "el", "fin", "gar", "horn"];
            Random random = new Random();

            List<string> names = new List<string>();
            for (int i = 0; i < count; i++)
            {
                string name = prefixes[random.Next(prefixes.Length)] +
                             suffixes[random.Next(suffixes.Length)];
                names.Add(name);
            }
            return names;
        }
    }
}}
