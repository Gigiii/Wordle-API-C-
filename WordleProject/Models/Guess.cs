namespace WordleProject.Models
{
    public class Guess
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }

        public string Word { get; set; } = string.Empty;
        public int GuessNumber { get; set; }
        public string GuessResult { get; set; } = string.Empty;
    }

}
