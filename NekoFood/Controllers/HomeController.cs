using Microsoft.AspNetCore.Mvc;
using NekoFood.Models;
using System.Diagnostics;

namespace NekoFood.Controllers
{
    [AuthorizeManager]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            bool isGetAll = true;
            return RedirectToRoute(new { controller = "BentoGroup", action = "Index", isGetAll });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}