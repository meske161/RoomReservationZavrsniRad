using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Models
{
    /// <summary>
    /// Prostorija za probe bendova.
    /// </summary>
    public class Room
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Naziv ili broj prostorije.
        /// </summary>
        [Required]
        public required string Name { get; set; }

        /// <summary>
        /// Opis prostorije.
        /// </summary>
        public string? Description { get; set; }
    }
}
