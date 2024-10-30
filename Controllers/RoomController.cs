using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomReservation.Data;
using RoomReservation.Models;

namespace RoomReservation.Controllers
{
    /// <summary>
    /// API za upravljanje prostorijama.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        /// <summary>
        /// Dohvaća sve prostorije.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            return await _context.Rooms.ToListAsync();
        }

        /// <summary>
        /// Dohvaća prostoriju prema ID-u.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return NotFound();

            return room;
        }

        /// <summary>
        /// Kreira novu prostoriju.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Room>> PostRoom([FromBody] Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
        }

        /// <summary>
        /// Ažurira postojeću prostoriju.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, [FromBody] Room room)
        {
            if (id != room.Id)
                return BadRequest();

            _context.Entry(room).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Briše prostoriju prema ID-u.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return NotFound();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
