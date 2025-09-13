using System.ComponentModel.DataAnnotations;
using EnglishLearningPlatform.Models;
namespace EnglishLearningPlatform.ViewModels {
    public class CreateWordViewModel {
        [Required(ErrorMessage = "Английское слово обязательно")]
        public string EnglishWord { get; set; }

        public string? Translation { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsLearned { get; set; }
        public string? Transcription { get; set; } = "";
        public string? Example { get; set; } = "";
        public string? PartOfSpeech { get; set; } = "";
        public int DifficultyLevel { get; set; } = 1;
        public string? WordListText { get; set; }
        public IFormFile? WordListFile { get; set; }
        public List<Word>? ExtractedWords { get; set; }

        // Для хранения выбранных индексов
        public List<int>? SelectedWordIndexes { get; set; }
        // Для проверки существования
        public bool WordExists { get; set; }
        public int? ExistingWordId { get; set; }
        public string? ExistingWordTranslation { get; set; }
        public string? ActionChoice { get; set; } // "view", "replace", "create_new"

        // Для списка слов
        public Dictionary<int, string>? WordActions { get; set; } // индекс -> действие
    }
}