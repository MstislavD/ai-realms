namespace AI
{
    public interface IAIService
    {
        Task<string> GenerateContentAsync(string prompt, string context = "");
        Task<string> GenerateNamesAsync(string race, string culture, int count);
    }
}

