namespace WordleProject.DTOs.Guesses
{
    public class GuessDTO
    {
        public int GuessNumber { get; set; }
        public string Word { get; set; } = string.Empty;
        public string GuessResult { get; set; } = string.Empty;
    }
}
