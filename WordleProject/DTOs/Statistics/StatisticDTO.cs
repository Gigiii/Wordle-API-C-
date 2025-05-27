namespace WordleProject.DTOs.Statistics
{
    public class StatisticDTO
    {
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int CurrentStreak { get; set; }
        public int MaxStreak { get; set; }
        public int TotalPoints { get; set; }
    }
}
