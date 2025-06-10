using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalProjectTest.Data;
using FinalProjectTest.Models;

namespace FinalProjectTest.Controllers
{
    [Route("api/locations")]
    [ApiController]
    public class LocationsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LocationsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/locations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocations([FromQuery] string? category)
        {
            var query = _context.Locations
                .Include(l => l.Images)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(l => l.Category == category);

            return Ok(await query.ToListAsync());
        }

        // GET: api/locations/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Location>> GetLocation(int id)
        {
            var location = await _context.Locations
                .Include(l => l.Images)
                .FirstOrDefaultAsync(l => l.LocationID == id);

            if (location == null)
                return NotFound();

            return location;
        }

        // ========== CATEGORY ENDPOINTS ==========

        [HttpGet("churches")] public Task<IActionResult> GetChurches() => GetByCategory("Church");
        [HttpGet("fortresses")] public Task<IActionResult> GetFortresses() => GetByCategory("Fortress");
        [HttpGet("fountains")] public Task<IActionResult> GetFountains() => GetByCategory("Fountain");
        [HttpGet("historical")] public Task<IActionResult> GetHistorical() => GetByCategory("Historical");
        [HttpGet("hotels")] public Task<IActionResult> GetHotels() => GetByCategory("Hotel");
        [HttpGet("markets")] public Task<IActionResult> GetMarkets() => GetByCategory("Market");
        [HttpGet("mosques")] public Task<IActionResult> GetMosques() => GetByCategory("Mosque");
        [HttpGet("museums")] public Task<IActionResult> GetMuseums() => GetByCategory("Museum");
        [HttpGet("palaces")] public Task<IActionResult> GetPalaces() => GetByCategory("Palace");
        [HttpGet("schools")] public Task<IActionResult> GetSchools() => GetByCategory("School");
        [HttpGet("shrines")] public Task<IActionResult> GetShrines() => GetByCategory("Shrine");
        [HttpGet("restaurants")] public Task<IActionResult> GetRestaurants() => GetByCategory("Restaurant");
        [HttpGet("cafes")] public Task<IActionResult> GetCafes() => GetByCategory("Cafe");

        // ========== PRIVATE HELPER ==========
        private async Task<IActionResult> GetByCategory(string category)
        {
            var results = await _context.Locations
                .Include(l => l.Images)
                .Where(l => l.Category == category)
                .ToListAsync();

            return Ok(results);
        }
    }
}
