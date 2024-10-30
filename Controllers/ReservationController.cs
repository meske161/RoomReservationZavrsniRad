using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomReservation.Data;
using RoomReservation.Models;

namespace RoomReservation.Controllers
{
    /// <summary>
    /// API za upravljanje rezervacijama.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;

        public ReservationController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Dohvaća sve rezervacije.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _context.Reservations
                .Include(r => r.Band)
                .Include(r => r.Room)
                .ToListAsync();
        }

        /// <summary>
        /// Dohvaća rezervaciju prema ID-u.
        /// </summary>
        /// <param name="id">ID rezervacije.</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Band)
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            return reservation;
        }

        /// <summary>
        /// Kreira novu rezervaciju.
        /// </summary>
        /// <param name="reservation">Objekt rezervacije koji se kreira.</param>
        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation([FromBody] Reservation reservation)
        {
            // Provjera postoji li bend s navedenim BandId
            var band = await _context.Bands.FindAsync(reservation.BandId);
            if (band == null)
            {
                return BadRequest("Bend s navedenim ID-om ne postoji.");
            }

            // Provjera postoji li prostorija s navedenim RoomId
            var room = await _context.Rooms.FindAsync(reservation.RoomId);
            if (room == null)
            {
                return BadRequest("Prostorija s navedenim ID-om ne postoji.");
            }

            // Provjera preklapanja rezervacija
            var overlappingReservation = await _context.Reservations
                .Where(r => r.RoomId == reservation.RoomId &&
                            ((reservation.StartTime >= r.StartTime && reservation.StartTime < r.EndTime) ||
                             (reservation.EndTime > r.StartTime && reservation.EndTime <= r.EndTime) ||
                             (reservation.StartTime <= r.StartTime && reservation.EndTime >= r.EndTime)))
                .FirstOrDefaultAsync();

            if (overlappingReservation != null)
            {
                return BadRequest("Termin je već rezerviran.");
            }

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            // Učitavanje navigacijskih svojstava
            _context.Entry(reservation).Reference(r => r.Band).Load();
            _context.Entry(reservation).Reference(r => r.Room).Load();

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
        }

        /// <summary>
        /// Ažurira postojeću rezervaciju.
        /// </summary>
        /// <param name="id">ID rezervacije koja se ažurira.</param>
        /// <param name="reservation">Objekt rezervacije s ažuriranim podacima.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, [FromBody] Reservation reservation)
        {
            if (id != reservation.Id)
                return BadRequest();

            // Provjera postoji li bend s navedenim BandId
            var band = await _context.Bands.FindAsync(reservation.BandId);
            if (band == null)
            {
                return BadRequest("Bend s navedenim ID-om ne postoji.");
            }

            // Provjera postoji li prostorija s navedenim RoomId
            var room = await _context.Rooms.FindAsync(reservation.RoomId);
            if (room == null)
            {
                return BadRequest("Prostorija s navedenim ID-om ne postoji.");
            }

            // Provjera preklapanja rezervacija (isključujući trenutnu rezervaciju)
            var overlappingReservation = await _context.Reservations
                .Where(r => r.RoomId == reservation.RoomId && r.Id != reservation.Id &&
                            ((reservation.StartTime >= r.StartTime && reservation.StartTime < r.EndTime) ||
                             (reservation.EndTime > r.StartTime && reservation.EndTime <= r.EndTime) ||
                             (reservation.StartTime <= r.StartTime && reservation.EndTime >= r.EndTime)))
                .FirstOrDefaultAsync();

            if (overlappingReservation != null)
            {
                return BadRequest("Termin je već rezerviran.");
            }

            _context.Entry(reservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Briše rezervaciju prema ID-u.
        /// </summary>
        /// <param name="id">ID rezervacije koja se briše.</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Dohvaća sve rezervacije za određenu prostoriju.
        /// </summary>
        /// <param name="roomId">ID prostorije.</param>
        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservationsByRoom(int roomId)
        {
            var reservations = await _context.Reservations
                .Where(r => r.RoomId == roomId)
                .Include(r => r.Band)
                .Include(r => r.Room)
                .ToListAsync();

            return reservations;
        }

        /// <summary>
        /// Provjerava postoji li rezervacija s navedenim ID-om.
        /// </summary>
        /// <param name="id">ID rezervacije.</param>
        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
