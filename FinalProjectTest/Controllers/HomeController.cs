using System.Diagnostics;
using FinalProjectTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Random featured locations
            var featured = await _context.Locations
                .OrderBy(r => Guid.NewGuid())
                .Take(6)
                .ToListAsync();

            // Top-rated hotels (Rating >= 9 from description text)
            var topHotels = await _context.Locations
                .Where(l => l.Category == "Hotel" && l.FullDescription.Contains("Rating"))
                .OrderByDescending(h => EF.Functions.Like(h.FullDescription, "%Rating: 9.%"))
                .Take(3)
                .ToListAsync();

            ViewBag.Featured = featured;
            ViewBag.TopHotels = topHotels;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
