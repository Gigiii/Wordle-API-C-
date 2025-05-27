using WordleProject.DTOs.Guesses;

namespace WordleProject.DTOs.Games
{
    public class GameDTO
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Attempts { get; set; }
        public bool IsWin { get; set; }
        public List<GuessDTO> Guesses { get; set; } = new();
        public string? TargetWord { get; set; }

    }
}
