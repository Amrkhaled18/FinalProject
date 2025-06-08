using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalProjectTest.Models;
using Microsoft.AspNetCore.Identity;

namespace FinalProjectTest.Controllers
{
    public class PreferencesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PreferencesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Preferences
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var pref = await _context.Preferences.FirstOrDefaultAsync(p => p.UserId == userId);

            if (pref == null)
                return RedirectToAction(nameof(Create));

            return View("Details", pref); // Or use View("Edit", pref);
        }

        // GET: Preferences/Details/5
        public async Task<IActionResult> Details()
        {
            var userId = _userManager.GetUserId(User);
            var preference = await _context.Preferences.FirstOrDefaultAsync(p => p.UserId == userId);
            if (preference == null)
            {
                return NotFound();
            }

            return View(preference);
        }

        // GET: Preferences/Create
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            var existing = await _context.Preferences.FirstOrDefaultAsync(p => p.UserId == userId);
            if (existing != null)
                return RedirectToAction(nameof(Edit));

            return View();
        }

        // POST: Preferences/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DietaryRestrictions,FavoriteCuisines,AccessibilityRequirements")] Preference preference)
        {
            var userId = _userManager.GetUserId(User);
            preference.UserId = userId;

            if (ModelState.IsValid)
            {
                _context.Add(preference);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(preference);
        }

        // GET: Preferences/Edit
        public async Task<IActionResult> Edit()
        {
            var userId = _userManager.GetUserId(User);
            var preference = await _context.Preferences.FirstOrDefaultAsync(p => p.UserId == userId);
            if (preference == null)
            {
                return RedirectToAction(nameof(Create));
            }
            return View(preference);
        }

        // POST: Preferences/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PreferenceID,DietaryRestrictions,FavoriteCuisines,AccessibilityRequirements")] Preference preference)
        {
            var userId = _userManager.GetUserId(User);
            preference.UserId = userId;

            if (id != preference.PreferenceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(preference);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PreferenceExists(preference.PreferenceID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(preference);
        }

        private bool PreferenceExists(int id)
        {
            return _context.Preferences.Any(e => e.PreferenceID == id);
        }
    }
}
