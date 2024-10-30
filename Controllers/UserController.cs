using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomReservation.Data;
using RoomReservation.Models;

namespace RoomReservation.Controllers
{
    /// <summary>
    /// API za upravljanje korisnicima.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        /// <summary>
        /// Dohvaća sve korisnike.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.Include(u => u.Band).ToListAsync();
        }

        /// <summary>
        /// Dohvaća korisnika prema ID-u.
        /// </summary>
        /// <param name="id">ID korisnika.</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.Include(u => u.Band).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();

            return user;
        }

        /// <summary>
        /// Kreira novog korisnika.
        /// </summary>
        /// <param name="user">Objekt korisnika koji se kreira.</param>
        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] User user)
        {
            // Provjera postoji li bend s navedenim BandId
            var band = await _context.Bands.FindAsync(user.BandId);
            if (band == null)
            {
                return BadRequest("Bend s navedenim ID-om ne postoji.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Učitavanje navigacijskog svojstva
            _context.Entry(user).Reference(u => u.Band).Load();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        /// <summary>
        /// Ažurira postojećeg korisnika.
        /// </summary>
        /// <param name="id">ID korisnika koji se ažurira.</param>
        /// <param name="user">Objekt korisnika s ažuriranim podacima.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest();

            // Provjera postoji li bend s navedenim BandId
            var band = await _context.Bands.FindAsync(user.BandId);
            if (band == null)
            {
                return BadRequest("Bend s navedenim ID-om ne postoji.");
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Briše korisnika prema ID-u.
        /// </summary>
        /// <param name="id">ID korisnika koji se briše.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Provjerava postoji li korisnik s navedenim ID-om.
        /// </summary>
        /// <param name="id">ID korisnika.</param>
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
