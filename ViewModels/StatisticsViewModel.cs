namespace EnglishLearningPlatform.ViewModels {
    public class StatisticsViewModel {
        public int TotalWords { get; set; }
        public int LearnedWords { get; set; }
        public int ProgressPercentage { get; set; }
        public int StreakDays { get; set; }
        public Dictionary<string, LevelStats> LevelDistribution { get; set; } = new();
    }

    public class LevelStats {
        public int Total { get; set; }
        public int Learned { get; set; }
    }
    public class ActivityDay {
        public DateTime Date { get; set; }
        public int WordCount { get; set; }
        public bool IsActive { get; set; }
    }
}