namespace WordleProject.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public string TargetWord { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Attempts { get; set; }
        public bool IsWin { get; set; }

        public ICollection<Guess> Guesses { get; set; }
    }

}
