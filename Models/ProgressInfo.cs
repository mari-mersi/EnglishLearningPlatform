namespace EnglishLearningPlatform.Models {
    public class ProgressInfo {
        public int WordsLearnedThisWeek { get; set; }
        public int WeeklyGoal { get; set; }
        public int ProgressPercentage => WeeklyGoal > 0 ? WordsLearnedThisWeek * 100 / WeeklyGoal : 0;
    }
}
