using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalProjectTest.Models;

namespace FinalProjectTest.Controllers
{
    public class LocationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Locations
        public async Task<IActionResult> Index(string search, string category)
        {
            var locations = _context.Locations
            .Include(l => l.Images) // 🛠 Include images
            .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                locations = locations.Where(l =>
                    l.Name.Contains(search) ||
                    l.ShortDescription.Contains(search) ||
                    l.Category.Contains(search));
            }

            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                locations = locations.Where(l => l.Category == category);
            }

            ViewBag.Categories = await _context.Locations
                .Select(l => l.Category)
                .Distinct()
                .ToListAsync();

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategory = category;

            // 🔥 Load user favorites
            var userId = User.Identity.IsAuthenticated ? _context.Users
                .FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id : null;

            var favoriteIds = new List<int>();
            if (userId != null)
            {
                favoriteIds = await _context.Favorites
                    .Where(f => f.UserID == userId)
                    .Select(f => f.LocationID)
                    .ToListAsync();
            }

            ViewBag.FavoriteLocationIds = favoriteIds;

            return View(await locations.ToListAsync());
        }
        public async Task<IActionResult> Map()
        {
            var locations = await _context.Locations
                .Where(l => !string.IsNullOrEmpty(l.Address)) // optional
                .ToListAsync();

            return View(locations);
        }



        // GET: Locations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var location = await _context.Locations
                .Include(l => l.Images) // 🛠 INCLUDE images
                .FirstOrDefaultAsync(m => m.LocationID == id);

            if (location == null)
                return NotFound();

            return View(location);
        }


        // GET: Locations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocationID,Name,Address,Category,Attributes,ProximityScore,ImageURL,DetailURL,VisitingHours,ShortDescription,FullDescription")] Location location)
        {
            if (ModelState.IsValid)
            {
                _context.Add(location);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // GET: Locations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LocationID,Name,Address,Category,Attributes,ProximityScore,ImageURL,DetailURL,VisitingHours,ShortDescription,FullDescription")] Location location)
        {
            if (id != location.LocationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(location);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.LocationID))
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
            return View(location);
        }

        // GET: Locations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .FirstOrDefaultAsync(m => m.LocationID == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location != null)
            {
                _context.Locations.Remove(location);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(e => e.LocationID == id);
        }
    }
}
