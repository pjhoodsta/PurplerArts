using Microsoft.AspNetCore.Mvc;
using PurplerArtsWeb.Models;
using PurplerArtsWeb.Models.SubmissionApplication;
using System.Diagnostics;

namespace PurplerArtsWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        [BindProperty]
        public SubmissionApplication SubmissionModel { get; set; } = null!;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext db)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SubmissionApplication()
        {
            SubmissionModel = new SubmissionApplication();
            return View(SubmissionModel);
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}