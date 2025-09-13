using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EnglishLearningPlatform.Data;
using EnglishLearningPlatform.Models;

namespace EnglishLearningPlatform.Controllers {
    public class TrainerController : Controller {
        private readonly AppDbContext _context;

        public TrainerController(AppDbContext context) {
            _context = context;
        }

        // GET: Trainer - страница тренажера
        public async Task<IActionResult> Index() {
            var unlearnedWords = await _context.Words
                .Where(w => !w.IsLearned)
                .ToListAsync();

            if (!unlearnedWords.Any()) {
                ViewBag.Message = "Все слова изучены! Добавьте новые слова или отметьте некоторые как невыученные.";
                return View();
            }

            // Выбираем случайное слово для тренировки
            var random = new Random();
            var wordToLearn = unlearnedWords[random.Next(unlearnedWords.Count)];

            ViewBag.WordToLearn = wordToLearn;
            return View();
        }

        // POST: Trainer/CheckAnswer
        [HttpPost]
        public async Task<IActionResult> CheckAnswer(int wordId, string userAnswer) {
            var word = await _context.Words.FindAsync(wordId);

            if (word == null) {
                return NotFound();
            }

            // Простая проверка ответа (можно улучшить)
            var isCorrect = word.Translation.ToLower().Trim() == userAnswer.ToLower().Trim();

            if (isCorrect) {
                word.IsLearned = true;
                _context.Update(word);
                await _context.SaveChangesAsync();
            }

            return Json(new { isCorrect, correctAnswer = word.Translation });
        }
    }
}