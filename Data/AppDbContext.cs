using Microsoft.EntityFrameworkCore;
using EnglishLearningPlatform.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EnglishLearningPlatform.Data {
    public class AppDbContext : IdentityDbContext<User> {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        }

        public DbSet<Word> Words { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder); // Важно вызвать базовый метод

            // Добавляем начальные данные через миграцию
            modelBuilder.Entity<Word>().HasData(
                new Word {
                    Id = 1,
                    EnglishWord = "Hello",
                    Translation = "Привет",
                    ImageUrl = "/images/hello.jpg",
                    IsLearned = false,
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Transcription = "/həˈloʊ/",
                    Example = "Hello, how are you?",
                    PartOfSpeech = "interjection",
                    DifficultyLevel = 1,
                    LastReviewed = null,
                    ReviewCount = 0,
                    ConfidenceScore = 0
                },
                new Word {
                    Id = 2,
                    EnglishWord = "World",
                    Translation = "Мир",
                    ImageUrl = "/images/world.jpg",
                    IsLearned = false,
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Transcription = "/wɜːrld/",
                    Example = "The world is beautiful",
                    PartOfSpeech = "noun",
                    DifficultyLevel = 1,
                    LastReviewed = null,
                    ReviewCount = 0,
                    ConfidenceScore = 0
                },
                new Word {
                    Id = 3,
                    EnglishWord = "Cat",
                    Translation = "Кошка",
                    ImageUrl = "/images/cat.jpg",
                    IsLearned = false,
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Transcription = "/kæt/",
                    Example = "The cat is sleeping",
                    PartOfSpeech = "noun",
                    DifficultyLevel = 1,
                    LastReviewed = null,
                    ReviewCount = 0,
                    ConfidenceScore = 0
                },
                new Word {
                    Id = 4,
                    EnglishWord = "Dog",
                    Translation = "Собака",
                    ImageUrl = "/images/dog.jpg",
                    IsLearned = false,
                    CreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Transcription = "/dɒɡ/",
                    Example = "The dog is barking",
                    PartOfSpeech = "noun",
                    DifficultyLevel = 1,
                    LastReviewed = null,
                    ReviewCount = 0,
                    ConfidenceScore = 0
                }
            );
        }
    }
}