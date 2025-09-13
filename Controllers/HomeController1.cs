using Microsoft.AspNetCore.Mvc;
using EnglishLearningPlatform.Models;
using System.Diagnostics;
using EnglishLearningPlatform.ViewModels;
using EnglishLearningPlatform.Data;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningPlatform.Controllers {
    public class HomeController1 : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        public HomeController1(ILogger<HomeController> logger, AppDbContext context) {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Главная страница сайта
        /// </summary>
        /// <returns></returns>
        public IActionResult Index() {
            return View();
        }

        /// <summary>
        /// Статистика изучения
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Statistics() {
            try {
                var statistics = await GetStatisticsData();
                var activityData = GenerateActivityData();

                return View(new StatisticsViewModel {
                    TotalWords = statistics.TotalWords,
                    LearnedWords = statistics.LearnedWords,
                    ProgressPercentage = statistics.ProgressPercentage,
                    StreakDays = statistics.StreakDays,
                    LevelDistribution = statistics.LevelDistribution
                });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Ошибка при получении статистики");

                // Возвращаем представление с пустыми данными
                return View(new StatisticsViewModel {
                    TotalWords = 0,
                    LearnedWords = 0,
                    ProgressPercentage = 0,
                    StreakDays = 0,
                    LevelDistribution = new Dictionary<string, LevelStats>()
                });
            }
        }

        /// <summary>
        /// Получение статистических данных из базы
        /// </summary>
        private async Task<StatisticsViewModel> GetStatisticsData() {
            try {
                var totalWords = await _context.Words.CountAsync();
                var learnedWords = await _context.Words.CountAsync(w => w.IsLearned);
                var progressPercentage = totalWords > 0 ? (int)((double)learnedWords / totalWords * 100) : 0;

                var levelDistribution = await _context.Words
                    .GroupBy(w => w.DifficultyLevel)
                    .ToDictionaryAsync(
                        g => GetLevelName(g.Key),
                        g => new LevelStats {
                            Total = g.Count(),
                            Learned = g.Count(w => w.IsLearned)
                        }
                    );

                var streak = CalculateStreak();

                return new StatisticsViewModel {
                    TotalWords = totalWords,
                    LearnedWords = learnedWords,
                    ProgressPercentage = progressPercentage,
                    StreakDays = streak,
                    LevelDistribution = levelDistribution
                };
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Ошибка при получении статистических данных");
                throw;
            }
        }

        /// <summary>
        /// Расчет текущей серии дней подряд
        /// </summary>
        private int CalculateStreak() {
            // Здесь можно реализовать логику расчета серии дней
            // Пока используем статическое значение для примера
            return 28;
        }

        /// <summary>
        /// Получение названия уровня по числу
        /// </summary>
        private string GetLevelName(int level) {
            return level switch {
                1 => "Начальный",
                2 => "Средний",
                3 => "Продвинутый",
                _ => $"Уровень {level}"
            };
        }

        /// <summary>
        /// Генерация данных активности (можно заменить на реальные из БД)
        /// </summary>
        private List<ActivityDay> GenerateActivityData() {
            var activities = new List<ActivityDay>();
            var random = new Random();
            var today = DateTime.Today;

            for (int i = 29; i >= 0; i--) {
                var date = today.AddDays(-i);
                var isActive = random.Next(0, 3) > 0;
                var wordCount = isActive ? random.Next(5, 21) : 0;

                activities.Add(new ActivityDay {
                    Date = date,
                    WordCount = wordCount,
                    IsActive = isActive
                });
            }

            return activities;
        }

        #region dontUse
        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Index1() {
            return View();
        }

        public IActionResult Homepage1() {
            return View();
        }

        public IActionResult Homepage2() {
            return View();
        }

        public IActionResult Homepage3() {
            return View();
        }
        #endregion
    }
}