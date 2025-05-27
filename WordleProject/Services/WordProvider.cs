namespace WordleProject.Services;

public static class WordProvider
{
    private static readonly string[] _words;

    static WordProvider()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "Words.txt");
        _words = File.ReadAllLines(path)
                     .Where(w => w.Length == 5)
                     .Select(w => w.ToLower())
                     .ToArray();
    }

    public static string GetRandomWord()
    {
        var random = new Random();
        return _words[random.Next(_words.Length)];
    }
}
