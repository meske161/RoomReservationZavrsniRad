using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomReservation.Data;
using RoomReservation.Models;

namespace RoomReservation.Controllers
{
    /// <summary>
    /// API za upravljanje bendovima.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BandController : Controller
    {
        private readonly AppDbContext _context;

        public BandController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Dohvaća sve bendove.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Band>>> GetBands()
        {
            return await _context.Bands.Include(b => b.Users).ToListAsync();
        }

        /// <summary>
        /// Dohvaća bend prema ID-u.
        /// </summary>
        /// <param name="id">ID benda.</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<Band>> GetBand(int id)
        {
            var band = await _context.Bands.Include(b => b.Users).FirstOrDefaultAsync(b => b.Id == id);
            if (band == null)
                return NotFound();

            return band;
        }

        /// <summary>
        /// Kreira novi bend.
        /// </summary>
        /// <param name="band">Objekt benda koji se kreira.</param>
        [HttpPost]
        public async Task<ActionResult<Band>> PostBand([FromBody] Band band)
        {
            _context.Bands.Add(band);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBand), new { id = band.Id }, band);
        }

        /// <summary>
        /// Ažurira postojeći bend.
        /// </summary>
        /// <param name="id">ID benda koji se ažurira.</param>
        /// <param name="band">Objekt benda s ažuriranim podacima.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBand(int id, [FromBody] Band band)
        {
            if (id != band.Id)
                return BadRequest();

            _context.Entry(band).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BandExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Briše bend prema ID-u.
        /// </summary>
        /// <param name="id">ID benda koji se briše.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBand(int id)
        {
            var band = await _context.Bands.FindAsync(id);
            if (band == null)
                return NotFound();

            _context.Bands.Remove(band);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Provjerava postoji li bend s navedenim ID-om.
        /// </summary>
        /// <param name="id">ID benda.</param>
        private bool BandExists(int id)
        {
            return _context.Bands.Any(e => e.Id == id);
        }
    }
}
