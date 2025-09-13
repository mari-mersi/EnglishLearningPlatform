using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EnglishLearningPlatform.Models {
    public class Word {
        public int Id { get; set; }
        [Required]
        public string EnglishWord { get; set; } = string.Empty;
        public string? Translation { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsLearned { get; set; }
        public bool NeedsTranslation { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Transcription { get; set; }
        public string? Example { get; set; } 
        public string? PartOfSpeech { get; set; }
        public int DifficultyLevel { get; set; } = 1;
        public DateTime? LastReviewed { get; set; }
        public int ReviewCount { get; set; }
        public double ConfidenceScore { get; set; }
        public bool IsSameWord(string englishWord) {
            return EnglishWord.Trim().Equals(englishWord.Trim(), StringComparison.OrdinalIgnoreCase);
        }
        [NotMapped] // Это свойство не будет сохраняться в БД
        public bool ExistsInDatabase { get; set; }
    }
}