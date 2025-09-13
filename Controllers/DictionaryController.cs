using System.Text.RegularExpressions;
using EnglishLearningPlatform.Data;
using EnglishLearningPlatform.Models;
using EnglishLearningPlatform.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace EnglishLearningPlatform.Controllers {
    public class DictionaryController : Controller {
        private readonly AppDbContext _context;
        public DictionaryController(AppDbContext context) {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool WordExists(int id) {
            return _context.Words.Any(e => e.Id == id);
        }

        /// <summary>
        /// Cловарь
        /// </summary>
        /// <returns>Dictionary/</returns>
        [HttpGet]
        public async Task<IActionResult> Index() {
            return View(await _context.Words.ToListAsync());
        }

        /// <summary>
        /// Карточка слова
        /// </summary>
        /// <param name="id">ИД слова</param>
        /// <returns>
        /// NotFound() - в случае, если ИД слова = null или само слово = null
        /// Dictionary/Details/ИД_слова - в остальных случаях
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var word = await _context.Words
                .FirstOrDefaultAsync(m => m.Id == id);
            if (word == null) {
                return NotFound();
            }

            return View(word);
        }

        #region Формы

        /// <summary>
        /// Форма добавления слова
        /// </summary>
        /// <returns>Dictionary/Create</returns>
        [HttpGet]
        public IActionResult Create() {
            var viewModel = new CreateWordViewModel();
            return View(viewModel);
        }

        /// <summary>
        /// Форма добавления слова
        /// </summary>
        /// <param name="viewModel">Модель для создания слова</param>
        /// <returns>
        /// Dictionary/Create - если не заполнено обязательное поле EnglishWord
        /// Dictionary/Index - в остальных случаях
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateWordViewModel viewModel) {
            // Проверяем обязательное поле
            if (string.IsNullOrWhiteSpace(viewModel.EnglishWord)) {
                ModelState.AddModelError("EnglishWord", "Английское слово обязательно");
                return View(viewModel);
            }

            // Проверяем существование слова
            var existingWord = await _context.Words
                .FirstOrDefaultAsync(w => w.EnglishWord.ToLower() == viewModel.EnglishWord.Trim().ToLower());

            if (existingWord != null) {
                viewModel.WordExists = true;
                viewModel.ExistingWordId = existingWord.Id;
                viewModel.ExistingWordTranslation = existingWord.Translation;
                return View(viewModel);
            }

            // Если слова нет, создаем новое
            bool needsTranslation = string.IsNullOrWhiteSpace(viewModel.Translation);

            var word = new Word {
                EnglishWord = viewModel.EnglishWord.Trim(),
                Translation = viewModel.Translation?.Trim(),
                ImageUrl = viewModel.ImageUrl?.Trim(),
                IsLearned = viewModel.IsLearned,
                Transcription = viewModel.Transcription?.Trim(),
                Example = viewModel.Example?.Trim(),
                PartOfSpeech = viewModel.PartOfSpeech?.Trim(),
                DifficultyLevel = viewModel.DifficultyLevel,
                CreatedDate = DateTime.Now,
                NeedsTranslation = needsTranslation
            };

            _context.Add(word);
            await _context.SaveChangesAsync();

            TempData["Message"] = needsTranslation
                ? "Слово добавлено в список для редактирования (ожидает перевода)"
                : "Слово успешно добавлено в словарь";

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CreateWordsList() {
            var viewModel = new CreateWordViewModel();
            return View(viewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWordsList(CreateWordViewModel viewModel) {
            try {
                if (!string.IsNullOrEmpty(viewModel.WordListText) || viewModel.WordListFile != null) {
                    var extractedWords = ExtractWordsFromText(viewModel);

                    // Проверяем существующие слова
                    var existingWords = await _context.Words
                        .Where(w => extractedWords.Select(ew => ew.EnglishWord.ToLower())
                            .Contains(w.EnglishWord.ToLower()))
                        .ToListAsync();

                    // Помечаем слова, которые уже существуют
                    foreach (var word in extractedWords) {
                        word.ExistsInDatabase = existingWords
                            .Any(ew => ew.IsSameWord(word.EnglishWord));
                    }

                    if (extractedWords.Any()) {
                        viewModel.ExtractedWords = extractedWords;
                        return View("WordListReview", viewModel);
                    }
                }
            }
            catch (Exception ex) {
                ModelState.AddModelError("WordListFile", ex.Message);
            }

            return View(viewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="selectedWordIndexes"></param>
        /// <param name="wordActions"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveWordsList(CreateWordViewModel viewModel, List<int> selectedWordIndexes, Dictionary<int, string> wordActions) {
            if (viewModel.ExtractedWords != null && selectedWordIndexes != null) {
                int addedCount = 0;
                int updatedCount = 0;

                // Сначала получаем все существующие слова для проверки
                var allExistingWords = await _context.Words.ToListAsync();

                foreach (var index in selectedWordIndexes) {
                    if (index >= 0 && index < viewModel.ExtractedWords.Count) {
                        var newWord = viewModel.ExtractedWords[index];
                        var action = wordActions.ContainsKey(index) ? wordActions[index] : "create";

                        // Проверяем существующее слово (используем клиентскую оценку)
                        var existingWord = allExistingWords
                            .FirstOrDefault(w => w.EnglishWord.Trim().Equals(newWord.EnglishWord.Trim(), StringComparison.OrdinalIgnoreCase));

                        if (action == "replace" && existingWord != null) {
                            // Обновляем существующее слово
                            if (!string.IsNullOrWhiteSpace(newWord.Translation))
                                existingWord.Translation = newWord.Translation;

                            existingWord.ImageUrl = newWord.ImageUrl;
                            existingWord.IsLearned = newWord.IsLearned;
                            existingWord.Transcription = newWord.Transcription;
                            existingWord.Example = newWord.Example;
                            existingWord.PartOfSpeech = newWord.PartOfSpeech;
                            existingWord.DifficultyLevel = newWord.DifficultyLevel;
                            existingWord.NeedsTranslation = string.IsNullOrWhiteSpace(newWord.Translation);

                            _context.Update(existingWord);
                            updatedCount++;
                        }
                        else if (action == "create" && existingWord == null) {
                            // Создаем новое слово
                            newWord.NeedsTranslation = string.IsNullOrWhiteSpace(newWord.Translation);
                            newWord.CreatedDate = DateTime.Now;
                            _context.Add(newWord);
                            addedCount++;
                        }
                        else if (action == "create" && existingWord != null) {
                            // Если слово существует, но action = create, создаем новое с другим именем
                            // Добавляем суффикс чтобы избежать конфликта
                            newWord.EnglishWord = $"{newWord.EnglishWord}_{DateTime.Now:yyyyMMddHHmmss}";
                            newWord.NeedsTranslation = string.IsNullOrWhiteSpace(newWord.Translation);
                            newWord.CreatedDate = DateTime.Now;
                            _context.Add(newWord);
                            addedCount++;
                        }
                    }
                }

                await _context.SaveChangesAsync();

                string message = "";
                if (addedCount > 0)
                    message += $"Добавлено {addedCount} новых слов. ";
                if (updatedCount > 0)
                    message += $"Обновлено {updatedCount} существующих слов. ";

                TempData["Message"] = message.Trim();
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = "Не удалось добавить слова";
            return RedirectToAction(nameof(CreateWordsList));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private List<Word> ExtractWordsFromText(CreateWordViewModel viewModel) {
            string text = string.Empty;

            if (!string.IsNullOrEmpty(viewModel.WordListText)) {
                text = viewModel.WordListText;
            }
            else if (viewModel.WordListFile != null) {
                var extension = Path.GetExtension(viewModel.WordListFile.FileName).ToLower();

                // Поддерживаем только TXT файлы
                if (extension == ".txt") {
                    using (var reader = new StreamReader(viewModel.WordListFile.OpenReadStream())) {
                        text = reader.ReadToEnd();
                    }
                }
                else {
                    throw new Exception("Поддерживаются только TXT файлы");
                }
            }

            // Извлекаем английские слова
            var wordPattern = new Regex(@"\b[a-zA-Z]{2,}\b");
            var words = new HashSet<string>();

            foreach (Match match in wordPattern.Matches(text)) {
                var word = match.Value.Trim().ToLower();
                if (word.Length >= 2) // Игнорируем слишком короткие слова
                {
                    words.Add(word);
                }
            }

            return words.OrderBy(w => w)
                       .Select(w => new Word {
                           EnglishWord = w,
                           Translation = string.Empty,
                           NeedsTranslation = true,
                           CreatedDate = DateTime.Now
                       })
                       .ToList();
        }

        /// <summary>
        /// Новый метод для обработки выбора действия при дубликате
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HandleDuplicate(CreateWordViewModel viewModel) {
            if (viewModel.ExistingWordId == null) {
                return RedirectToAction(nameof(Create));
            }

            var existingWord = await _context.Words.FindAsync(viewModel.ExistingWordId);
            if (existingWord == null) {
                return RedirectToAction(nameof(Create));
            }

            switch (viewModel.ActionChoice) {
                case "view":
                    return RedirectToAction(nameof(Details), new { id = viewModel.ExistingWordId });

                case "replace":
                    // Обновляем существующее слово
                    if (!string.IsNullOrWhiteSpace(viewModel.Translation))
                        existingWord.Translation = viewModel.Translation.Trim();

                    if (!string.IsNullOrWhiteSpace(viewModel.ImageUrl))
                        existingWord.ImageUrl = viewModel.ImageUrl.Trim();

                    existingWord.IsLearned = viewModel.IsLearned;

                    if (!string.IsNullOrWhiteSpace(viewModel.Transcription))
                        existingWord.Transcription = viewModel.Transcription.Trim();

                    if (!string.IsNullOrWhiteSpace(viewModel.Example))
                        existingWord.Example = viewModel.Example.Trim();

                    if (!string.IsNullOrWhiteSpace(viewModel.PartOfSpeech))
                        existingWord.PartOfSpeech = viewModel.PartOfSpeech.Trim();

                    existingWord.DifficultyLevel = viewModel.DifficultyLevel;
                    existingWord.NeedsTranslation = string.IsNullOrWhiteSpace(viewModel.Translation);

                    _context.Update(existingWord);
                    await _context.SaveChangesAsync();

                    TempData["Message"] = "Существующее слово обновлено";
                    return RedirectToAction(nameof(Details), new { id = viewModel.ExistingWordId });

                case "create_new":
                default:
                    // Создаем новое слово (пользователь ввел другое слово)
                    return RedirectToAction(nameof(Create));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string ReadDocxFile(IFormFile file) {
            using (var stream = file.OpenReadStream()) {
                using (WordprocessingDocument doc = WordprocessingDocument.Open(stream, false)) {
                    var body = doc.MainDocumentPart.Document.Body;
                    return body.InnerText;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CheckWordExists([FromBody] Word request) {
            if (string.IsNullOrWhiteSpace(request.EnglishWord)) {
                return Json(false);
            }

            // Получаем все слова и проверяем на клиенте
            var allWords = await _context.Words.ToListAsync();
            var exists = allWords.Any(w =>
                w.EnglishWord.Trim().Equals(request.EnglishWord.Trim(), StringComparison.OrdinalIgnoreCase));

            return Json(exists);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CheckWordDuplicates([FromBody] List<string> words) {
            if (words == null || !words.Any()) {
                return Json(new { duplicates = new List<string>() });
            }

            // Получаем все существующие слова
            var allWords = await _context.Words.ToListAsync();
            var duplicateWords = new List<string>();

            foreach (var word in words) {
                if (string.IsNullOrWhiteSpace(word))
                    continue;

                // Проверяем на существование в базе
                var existsInDb = allWords.Any(w =>
                    w.EnglishWord.Trim().Equals(word.Trim(), StringComparison.OrdinalIgnoreCase));

                // Проверяем на дубликаты в предоставленном списке
                var duplicateInList = words.Count(w =>
                    !string.IsNullOrWhiteSpace(w) &&
                    w.Trim().Equals(word.Trim(), StringComparison.OrdinalIgnoreCase)) > 1;

                if (existsInDb || duplicateInList) {
                    duplicateWords.Add(word.ToLower());
                }
            }

            return Json(new { duplicates = duplicateWords.Distinct().ToList() });
        }

        /// <summary>
        /// Получить данные существующих слов
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> GetExistingWordsData([FromBody] List<string> words) {
            if (words == null || !words.Any()) {
                return Json(new Dictionary<string, object>());
            }

            var existingWords = await _context.Words
                .Where(w => words.Contains(w.EnglishWord.ToLower()))
                .ToListAsync();

            var result = existingWords.ToDictionary(
                w => w.EnglishWord.ToLower(),
                w => new {
                    translation = w.Translation,
                    transcription = w.Transcription,
                    partOfSpeech = w.PartOfSpeech,
                    example = w.Example,
                    imageUrl = w.ImageUrl,
                    difficultyLevel = w.DifficultyLevel,
                    isLearned = w.IsLearned
                });

            return Json(result);
        }

        /// <summary>
        /// Форма редактирования слова
        /// </summary>
        /// <param name="id">ИД слова</param>
        /// <returns>
        /// NotFound() - в случае, если ИД слова = null или само слово = null 
        /// Dictionary/Edit/ИД_слова
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var word = await _context.Words.FindAsync(id);
            if (word == null) {
                return NotFound();
            }
            return View(word);
        }

        /// <summary>
        /// Форма редактирования слова
        /// </summary>
        /// <param name="id">ИД слова</param>
        /// <param name="word">Слово</param>
        /// <returns>
        /// NotFound() - в случае, если ИД не совпадает с ИД слова
        /// Dictionary/Index - в случае, если редактирование корректно
        /// Dictionary/Edit/ИД_слова - в остальных случаях
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EnglishWord,Translation,ImageUrl,IsLearned,CreatedDate,Transcription,Example,PartOfSpeech,DifficultyLevel,NeedsTranslation")] Word word) {
            if (id != word.Id) {
                return NotFound();
            }

            if (!WordExists(id)) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    // Обновляем статус необходимости перевода
                    word.NeedsTranslation = string.IsNullOrWhiteSpace(word.Translation);

                    _context.Update(word);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!WordExists(word.Id)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(word);
        }

        /// <summary>
        /// Фома удаления слова
        /// </summary>
        /// <param name="id">ИД слова</param>
        /// <returns>
        /// NotFound() - в случае, если ИД = null или слово = null
        /// Dictionary/Delete/ИД_слова - в остальных случаях
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var word = await _context.Words
                .FirstOrDefaultAsync(m => m.Id == id);
            if (word == null) {
                return NotFound();
            }

            return View(word);
        }

        /// <summary>
        /// Информация об удаленном слове
        /// </summary>
        /// <param name="id">ИД слова</param>
        /// <returns>
        /// NotFound() - если слова не существует
        /// Dictionary/Delete/ИД_слова 
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) {
            var word = await _context.Words.FindAsync(id);
            if (word == null) {
                return NotFound();
            }

            _context.Words.Remove(word);
            await _context.SaveChangesAsync();

            // Передаем удаленное слово в представление
            return View("DeleteConfirmed", word);
        }
        #endregion
    }
}
