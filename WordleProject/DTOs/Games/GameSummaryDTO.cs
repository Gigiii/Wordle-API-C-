namespace WordleProject.DTOs.Games
{
    public class GameSummaryDTO
    {
        public int Id { get; set; }
        public bool IsWin { get; set; }
        public int Attempts { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? TargetWord { get; set; }
    }
}
