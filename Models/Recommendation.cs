namespace EnglishLearningPlatform.Models {
    public class Recommendation {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public RecommendationType Type { get; set; }
        public string Source { get; set; }
        public string ImageUrl { get; set; }
    }

    public enum RecommendationType {
        Article,
        Video,
        Audio
    }
}
