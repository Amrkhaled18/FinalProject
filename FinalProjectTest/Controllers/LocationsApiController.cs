using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
        {
            var locations = await _context.Locations
                .Include(l => l.Images)
                .ToListAsync();

            return Ok(locations);
        }

        // GET: api/locations/5
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
    }
}
