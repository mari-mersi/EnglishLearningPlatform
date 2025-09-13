using EnglishLearningPlatform.Data;
using Microsoft.AspNetCore.Mvc;

namespace EnglishLearningPlatform.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        public HomeController(ILogger<HomeController> logger, AppDbContext context) {
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
        /// Страница политики конфиденциальности
        /// </summary>
        /// <returns></returns>
        public IActionResult Privacy() {
            return View();
        }
    }
}
